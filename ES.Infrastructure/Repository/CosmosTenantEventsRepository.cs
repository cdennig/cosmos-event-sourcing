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
    CosmosTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantEventsRepository<TTenantKey,
        TAggregate, TKey
        , TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly Container _eventsContainer;

    private readonly ITenantDomainEventsFactory<TTenantKey, TKey, TPrincipalKey> _domainEventsFactory;

    private readonly ITenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey>
        _tenantAggregateRootFactory;

    public CosmosTenantEventsRepository(Container eventsContainer,
        ITenantDomainEventsFactory<TTenantKey, TKey, TPrincipalKey> domainEventsFactory,
        ITenantAggregateRootFactory<TTenantKey,
            TAggregate, TKey, TPrincipalKey> tenantAggregateRootFactory)
    {
        _eventsContainer = eventsContainer;
        _domainEventsFactory = domainEventsFactory;
        _tenantAggregateRootFactory = tenantAggregateRootFactory;
    }

    public async Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        var domainEvents = aggregateRoot.DomainEvents;

        if (domainEvents.Count == 0) return;

        var pk = new PartitionKey(aggregateRoot.Id?.ToString());

        // const string sqlQueryText =
        //     "SELECT VALUE(MAX(c.version)) FROM c  WHERE c.type = @type AND c.tenantId = @tenantId";
        //
        // var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT")
        //     .WithParameter("@tenantId", aggregateRoot.TenantId.ToString());
        //
        // var fi = _eventsContainer.GetItemQueryIterator<long>(queryDefinition, null,
        //     new QueryRequestOptions { PartitionKey = pk });
        //
        // var maxVersion = -1L;
        // var nextVersion = domainEvents.First().Version;
        //
        // if (fi.HasMoreResults)
        // {
        //     var versionResponse = await fi.ReadNextAsync(cancellationToken);
        //     if (versionResponse.Count > 0)
        //     {
        //         maxVersion = versionResponse.Single();
        //     }
        // }
        //
        // if (maxVersion >= nextVersion) throw new ApplicationException("Version mismatch!");

        var tb = _eventsContainer.CreateTransactionalBatch(pk);
        foreach (var @event in domainEvents)
        {
            var ed = new TenantEventData<TTenantKey, TKey, TPrincipalKey>(@event);
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

    public async Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id,
        CancellationToken cancellationToken = default)
    {
        var pk = new PartitionKey(id.ToString());

        var events = new List<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>>();

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
            if (documents == null)
                throw new NullReferenceException("Got invalid response from event store.");
            events.AddRange(documents.Select(document =>
                _domainEventsFactory.BuildEvent(document["eventType"].ToString(),
                    (double)document["eventVersion"],
                    document["tenantId"].ToString(),
                    document["raisedBy"].ToString(),
                    document["aggregateId"].ToString(), document["aggregateType"].ToString(),
                    document["timestamp"].ToString(), (long)document["version"], (JObject)document["event"])));
        }

        var aggregateRoot = _tenantAggregateRootFactory.Create(tenantId, id, events);
        return aggregateRoot;
    }
}