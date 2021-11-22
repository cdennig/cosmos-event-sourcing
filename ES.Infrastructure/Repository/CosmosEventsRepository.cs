using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class CosmosEventsRepository<TTenantId, TA, TKey> : IEventsRepository<TTenantId, TA, TKey>
        where TA : class, IAggregateRoot<TTenantId, TKey>
    {
        private readonly Container _eventsContainer;

        private readonly List<Assembly> _assemblies = new();
        private readonly Dictionary<string, ConstructorInfo> _eventConstructors = new();

        public CosmosEventsRepository(Container eventsContainer)
        {
            _eventsContainer = eventsContainer;
        }

        public async Task AppendAsync(TA aggregateRoot)
        {
            var domainEvents = aggregateRoot.DomainEvents;
            var pk = new PartitionKey(aggregateRoot.Id.ToString());

            // _eventsContainer.GetItemQueryIterator<EventData<TKey>>(new QueryDefinition("SELECT "));

            var tb = _eventsContainer.CreateTransactionalBatch(pk);
            foreach (var @event in domainEvents)
            {
                var ed = new EventData<TTenantId, TKey>(@event);
                tb.CreateItem(ed, new TransactionalBatchItemRequestOptions
                {
                    EnableContentResponseOnWrite = false
                });
            }

            var tbResult = await tb.ExecuteAsync();
        }

        public async Task<TA> RehydrateAsync(TTenantId tenantId, TKey id)
        {
            var pk = new PartitionKey(id.ToString());

            var events = new List<IDomainEvent<TTenantId, TKey>>();

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
                using ResponseMessage response = await feedIterator.ReadNextAsync();
                using StreamReader sr = new(response.Content);
                using JsonTextReader jtr = new(sr);
                JObject result = await JObject.LoadAsync(jtr);
                var documents = result["Documents"];
                events.AddRange(documents.Select(document => DeserializeEvent(document["eventType"].ToString(),
                    document["tenantId"].ToString(),
                    document["aggregateId"].ToString(), document["aggregateType"].ToString(),
                    document["timestamp"].ToString(), (long)document["version"], (JObject)document["event"])));
            }

            var aggregateRoot = BaseAggregateRoot<TTenantId, TA, TKey>.Create(tenantId, id, events);
            return aggregateRoot;
        }

        private IDomainEvent<TTenantId, TKey> DeserializeEvent(string eventType, string tenantId, string aggregateId,
            string aggregateType,
            string timestamp, long version, JObject rawEvent)
        {
            var ci = GetConstructorInfo(eventType);
            var @event = ci.Invoke(new object[]
                {
                    aggregateType,
                    TypeDescriptor.GetConverter(typeof(TTenantId)).ConvertFromString(tenantId),
                    TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(aggregateId), 
                    version, 
                    DateTimeOffset.Parse(timestamp)
                }) as
                IDomainEvent<TTenantId, TKey>;
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

            var ci = eType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                new[] { typeof(string), typeof(TTenantId), typeof(TKey), typeof(long), typeof(DateTimeOffset) });

            _eventConstructors.Add(eventType, ci);

            return ci;
        }
    }
}