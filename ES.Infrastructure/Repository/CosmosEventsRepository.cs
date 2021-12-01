using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ES.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Repository;
using Container = Microsoft.Azure.Cosmos.Container;

namespace ES.Infrastructure.Repository
{
    public class
        CosmosEventsRepository<TTenantId, TA, TKey, TPrincipalId> : IEventsRepository<TTenantId, TA, TKey, TPrincipalId>
        where TA : class, IAggregateRoot<TTenantId, TKey, TPrincipalId>
    {
        private readonly Container _eventsContainer;

        private readonly List<Assembly> _assemblies = new();
        private readonly Dictionary<string, ConstructorInfo> _eventConstructors = new();

        public CosmosEventsRepository(Container eventsContainer)
        {
            _eventsContainer = eventsContainer;
        }

        public async Task AppendAsync(TA aggregateRoot, CancellationToken cancellationToken = default)
        {
            var domainEvents = aggregateRoot.DomainEvents;

            if (domainEvents.Count == 0) return;

            var pk = new PartitionKey(aggregateRoot.Id.ToString());

            const string sqlQueryText =
                "SELECT VALUE(MAX(c.version)) FROM c  WHERE c.type = @type AND c.tenantId = @tenantId";

            var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT")
                .WithParameter("@tenantId", aggregateRoot.TenantId.ToString());

            var fi = _eventsContainer.GetItemQueryIterator<long>(queryDefinition, null,
                new QueryRequestOptions {PartitionKey = pk});

            var maxVersion = -1L;
            var nextVersion = domainEvents.First().Version;

            if (fi.HasMoreResults)
            {
                var versionResponse = await fi.ReadNextAsync(cancellationToken);
                if (versionResponse.Count > 0)
                {
                    maxVersion = versionResponse.Single();
                }
            }

            if (maxVersion >= nextVersion) throw new ApplicationException("Version mismatch!");

            var tb = _eventsContainer.CreateTransactionalBatch(pk);
            foreach (var @event in domainEvents)
            {
                var ed = new EventData<TTenantId, TKey, TPrincipalId>(@event);
                tb.CreateItem(ed, new TransactionalBatchItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                });
            }

            var tbResult = await tb.ExecuteAsync(cancellationToken);

            if (!tbResult.IsSuccessStatusCode)
            {
                throw new ApplicationException("Unable to save events.");
            }
        }

        public async Task<TA> RehydrateAsync(TTenantId tenantId, TKey id, CancellationToken cancellationToken = default)
        {
            var pk = new PartitionKey(id.ToString());

            var events = new List<IDomainEvent<TTenantId, TKey, TPrincipalId>>();

            const string sqlQueryText =
                "SELECT * FROM c WHERE c.type = @type AND c.tenantId = @tenantId ORDER BY c.version ASC";

            var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT")
                .WithParameter("@tenantId", tenantId.ToString());

            var feedIterator =
                _eventsContainer.GetItemQueryStreamIterator(queryDefinition,
                    null, new QueryRequestOptions
                    {
                        PartitionKey = pk, MaxItemCount = 100
                    });
            while (feedIterator.HasMoreResults)
            {
                using ResponseMessage response = await feedIterator.ReadNextAsync(cancellationToken);
                using StreamReader sr = new(response.Content);
                using JsonTextReader jtr = new(sr);
                JObject result = await JObject.LoadAsync(jtr, cancellationToken);
                var documents = result["Documents"];
                events.AddRange(documents.Select(document => DeserializeEvent(document["eventType"].ToString(),
                    document["tenantId"].ToString(),
                    document["raisedBy"].ToString(),
                    document["aggregateId"].ToString(), document["aggregateType"].ToString(),
                    document["timestamp"].ToString(), (long) document["version"], (JObject) document["event"])));
            }

            var aggregateRoot = BaseAggregateRoot<TTenantId, TA, TKey, TPrincipalId>.Create(tenantId, id, events);
            return aggregateRoot;
        }

        private IDomainEvent<TTenantId, TKey, TPrincipalId> DeserializeEvent(string eventType, string tenantId,
            string raisedBy, string aggregateId,
            string aggregateType,
            string timestamp, long version, JObject rawEvent)
        {
            var ci = GetConstructorInfo(eventType);
            var @event = ci.Invoke(new object[]
                {
                    aggregateType,
                    TypeDescriptor.GetConverter(typeof(TTenantId)).ConvertFromString(tenantId),
                    TypeDescriptor.GetConverter(typeof(TPrincipalId)).ConvertFromString(raisedBy),
                    TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(aggregateId),
                    version,
                    DateTimeOffset.Parse(timestamp)
                }) as
                IDomainEvent<TTenantId, TKey, TPrincipalId>;
            JsonConvert.PopulateObject(rawEvent.ToString(), @event);
            return @event;
        }

        private ConstructorInfo GetConstructorInfo(string eventType)
        {
            if (_assemblies.Count == 0)
            {
                _assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            }

            if (_eventConstructors.ContainsKey(eventType))
            {
                return _eventConstructors[eventType];
            }

            var eType = _assemblies.Select(a => a.GetType(eventType, false))
                .FirstOrDefault(t => t != null) ?? Type.GetType(eventType);

            if (eType == null)
                throw new ArgumentException("Event type not found", eventType);

            var ci = eType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                new[]
                {
                    typeof(string), typeof(TTenantId), typeof(TPrincipalId), typeof(TKey), typeof(long),
                    typeof(DateTimeOffset)
                });

            _eventConstructors.Add(eventType, ci);

            return ci;
        }
    }
}