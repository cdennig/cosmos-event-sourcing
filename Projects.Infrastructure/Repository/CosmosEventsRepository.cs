using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Projects.Infrastructure.Data;
using Projects.Shared.Aggregate;
using Projects.Shared.Events;
using Projects.Shared.Repository;

namespace Projects.Infrastructure.Repository
{
    public class CosmosEventsRepository<TA, TKey> : IEventsRepository<TA, TKey>
        where TA : class, IAggregateRoot<TKey>
    {
        private readonly Container _eventsContainer;

        private Assembly[] _assemblies;

        private readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

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
                var ed = new EventData<TKey>(@event);
                tb.CreateItem(ed, new TransactionalBatchItemRequestOptions()
                {
                    EnableContentResponseOnWrite = false
                });
            }

            var tbResult = await tb.ExecuteAsync();
        }

        public async Task<TA> RehydrateAsync(TKey key)
        {
            var pk = new PartitionKey(key.ToString());

            var events = new List<IDomainEvent<TKey>>();

            const string sqlQueryText = "SELECT * FROM c WHERE c.type = @type ORDER BY c.version ASC";

            var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT");

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
                foreach (var document in documents)
                {
                    events.Add(DeserializeEvent(document["eventType"].ToString(), document["aggregateId"].ToString(),
                        document["aggregateType"].ToString(), document["timestamp"].ToString(),
                        (long) document["version"],
                        (JObject) document["event"]));
                }
            }

            var aggregateRoot = BaseAggregateRoot<TA, TKey>.Create(events);
            return aggregateRoot;
        }

        private IDomainEvent<TKey> DeserializeEvent(string eventType, string aggregateId, string aggregateType,
            string timestamp, long version, JObject rawEvent)
        {
            if (_assemblies == null)
            {
                _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            var eType = _assemblies.Select(a => a.GetType(eventType, false))
                .FirstOrDefault(t => t != null) ?? Type.GetType(eventType);

            var ci = eType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                new[] {typeof(string), typeof(TKey), typeof(long), typeof(DateTimeOffset)});
            var @event = ci.Invoke(new object[]
                    {aggregateType, Guid.Parse(aggregateId), version, DateTimeOffset.Parse(timestamp)}) as
                IDomainEvent<TKey>;
            JsonConvert.PopulateObject(rawEvent.ToString(), @event);
            // var @event =
            //     JsonConvert.DeserializeObject(rawEvent.ToString(), eType, JsonSerializerSettings) as IDomainEvent<TKey>;

            return @event;
        }
    }
}