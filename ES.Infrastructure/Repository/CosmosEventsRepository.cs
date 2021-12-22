using ES.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Repository;
using Container = Microsoft.Azure.Cosmos.Container;

namespace ES.Infrastructure.Repository;

public class
    CosmosEventsRepository<TAggregate, TKey, TPrincipalKey> : IEventsRepository<TAggregate, TKey
        , TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly Container _eventsContainer;

    private readonly IDomainEventsFactory<TKey, TPrincipalKey> _domainEventsFactory;

    public CosmosEventsRepository(Container eventsContainer,
        IDomainEventsFactory<TKey, TPrincipalKey> domainEventsFactory)
    {
        _eventsContainer = eventsContainer;
        _domainEventsFactory = domainEventsFactory;
    }

    public async Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        var domainEvents = aggregateRoot.DomainEvents;

        if (domainEvents.Count == 0) return;

        var pk = new PartitionKey(aggregateRoot.Id.ToString());

        const string sqlQueryText =
            "SELECT VALUE(MAX(c.version)) FROM c  WHERE c.type = @type";

        var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT");

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
            var ed = new EventData<TKey, TPrincipalKey>(@event);
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

    public async Task<TAggregate> RehydrateAsync(TKey id,
        CancellationToken cancellationToken = default)
    {
        var pk = new PartitionKey(id.ToString());

        var events = new List<IDomainEvent<TKey, TPrincipalKey>>();

        const string sqlQueryText =
            "SELECT * FROM c WHERE c.type = @type ORDER BY c.version ASC";

        var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT");

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
            events.AddRange(documents.Select(document =>
                _domainEventsFactory.BuildEvent(document["eventType"].ToString(),
                    (double) document["eventVersion"],
                    document["raisedBy"].ToString(),
                    document["aggregateId"].ToString(), document["aggregateType"].ToString(),
                    document["timestamp"].ToString(), (long) document["version"], (JObject) document["event"])));
        }

        var aggregateRoot =
            AggregateRoot<TAggregate, TKey, TPrincipalKey>.Create(id, events);
        return aggregateRoot;
    }
}