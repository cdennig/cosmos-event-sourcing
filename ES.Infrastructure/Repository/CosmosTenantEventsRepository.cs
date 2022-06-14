using ES.Infrastructure.Data;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Repository;
using Microsoft.Extensions.Logging;
using Container = Microsoft.Azure.Cosmos.Container;

namespace ES.Infrastructure.Repository;

public class
    CosmosTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantEventsRepository<TTenantKey,
        TAggregate, TKey
        , TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly Container _eventsContainer;

    private readonly ILogger<CosmosTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>> _logger;
    private readonly ITenantDomainEventsFactory<TTenantKey, TKey, TPrincipalKey> _domainEventsFactory;

    private readonly ITenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey>
        _tenantAggregateRootFactory;

    public CosmosTenantEventsRepository(Container eventsContainer,
        ITenantDomainEventsFactory<TTenantKey, TKey, TPrincipalKey> domainEventsFactory,
        ITenantAggregateRootFactory<TTenantKey,
            TAggregate, TKey, TPrincipalKey> tenantAggregateRootFactory,
        ILogger<CosmosTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>> logger)
    {
        _eventsContainer = eventsContainer;
        _domainEventsFactory = domainEventsFactory;
        _tenantAggregateRootFactory = tenantAggregateRootFactory;
        _logger = logger;
    }

    public async Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Appending {EventCount} domain events of aggregate root {AggregateRootId} / tenant {TenantId}",
            aggregateRoot.DomainEvents.Count, aggregateRoot.Id, aggregateRoot.TenantId);
        var domainEvents = aggregateRoot.DomainEvents;

        if (domainEvents.Count == 0)
        {
            _logger.LogWarning(
                "Tried to append domain events of aggregate root {AggregateRootId} / tenant {TenantId}, but DomainEvents was empty",
                aggregateRoot.Id, aggregateRoot.TenantId);
            return;
        }

        var pk = new PartitionKey(aggregateRoot.Id?.ToString());

        _logger.LogTrace(
            "Creating transactional batch for domain events of aggregate root {AggregateRootId} / tenant {TenantId}",
            aggregateRoot.Id, aggregateRoot.TenantId);
        var tb = _eventsContainer.CreateTransactionalBatch(pk);
        foreach (var @event in domainEvents)
        {
            var ed = new TenantEventData<TTenantKey, TKey, TPrincipalKey>(@event);
            tb.CreateItem(ed, new TransactionalBatchItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            });
        }

        _logger.LogTrace(
            "Executing transactional batch for domain events of aggregate root {AggregateRootId} / tenant {TenantId}",
            aggregateRoot.Id, aggregateRoot.TenantId);
        var tbResult = await tb.ExecuteAsync(cancellationToken);

        if (!tbResult.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Error appending domain events of aggregate root {AggregateRootId} / tenant {TenantId}. Error message: {ErrorMessage}",
                aggregateRoot.Id, aggregateRoot.TenantId, tbResult.ErrorMessage);
            throw new ApplicationException("Unable to save events.");
        }

        _logger.LogInformation(
            "Successfully appended domain events of aggregate root {AggregateRootId} / tenant {TenantId}. Clearing events now",
            aggregateRoot.Id, aggregateRoot.TenantId);
        // remove all events from the aggregate root
        aggregateRoot.ClearEvents();
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

        _logger.LogInformation("Rehydrating aggregate root {AggregateRootId} / tenant {TenantId}", id, tenantId);

        var feedIterator =
            _eventsContainer.GetItemQueryStreamIterator(queryDefinition,
                null, new QueryRequestOptions
                {
                    PartitionKey = pk, MaxItemCount = 100
                });

        _logger.LogInformation("Reading events for aggregate root {AggregateRootId} / tenant {TenantId}", id, tenantId);
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

        _logger.LogInformation("Got {EventCount} events for aggregate root {AggregateRootId} / tenant {TenantId}", tenantId, events.Count, id);

        var aggregateRoot = _tenantAggregateRootFactory.Create(tenantId, id, events);
        _logger.LogInformation("Aggregate root {AggregateRootId} / tenant {TenantId} rehydrated", id, tenantId);

        return aggregateRoot;
    }
}