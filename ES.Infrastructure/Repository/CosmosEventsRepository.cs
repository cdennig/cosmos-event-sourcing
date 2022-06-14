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
    CosmosEventsRepository<TAggregate, TKey, TPrincipalKey> : IEventsRepository<TAggregate, TKey
        , TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly Container _eventsContainer;

    private readonly ILogger<CosmosEventsRepository<TAggregate, TKey, TPrincipalKey>> _logger;
    private readonly IDomainEventsFactory<TKey, TPrincipalKey> _domainEventsFactory;
    private readonly IAggregateRootFactory<TAggregate, TKey, TPrincipalKey> _aggregateRootFactory;

    public CosmosEventsRepository(Container eventsContainer,
        IDomainEventsFactory<TKey, TPrincipalKey> domainEventsFactory,
        IAggregateRootFactory<TAggregate, TKey, TPrincipalKey> aggregateRootFactory,
        ILogger<CosmosEventsRepository<TAggregate, TKey, TPrincipalKey>> logger)
    {
        _eventsContainer = eventsContainer;
        _domainEventsFactory = domainEventsFactory;
        _aggregateRootFactory = aggregateRootFactory;
        _logger = logger;
    }

    public async Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Appending {EventCount} domain events of aggregate root {AggregateRootId}",
            aggregateRoot.DomainEvents.Count, aggregateRoot.Id);
        var domainEvents = aggregateRoot.DomainEvents;

        if (domainEvents.Count == 0)
        {
            _logger.LogWarning(
                "Tried to append domain events of aggregate root {AggregateRootId}, but DomainEvents was empty",
                aggregateRoot.Id);
            return;
        }

        var pk = new PartitionKey(aggregateRoot.Id?.ToString());

        _logger.LogTrace("Creating transactional batch for domain events of aggregate root {AggregateRootId}",
            aggregateRoot.Id);
        var tb = _eventsContainer.CreateTransactionalBatch(pk);
        foreach (var @event in domainEvents)
        {
            var ed = new EventData<TKey, TPrincipalKey>(@event);
            tb.CreateItem(ed, new TransactionalBatchItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            });
        }

        _logger.LogTrace("Executing transactional batch for domain events of aggregate root {AggregateRootId}",
            aggregateRoot.Id);
        var tbResult = await tb.ExecuteAsync(cancellationToken);

        if (!tbResult.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Error appending domain events of aggregate root {AggregateRootId}. Error message: {ErrorMessage}",
                aggregateRoot.Id, tbResult.ErrorMessage);
            throw new ApplicationException("Unable to save events.");
        }

        _logger.LogInformation(
            "Successfully appended domain events of aggregate root {AggregateRootId}. Clearing events now",
            aggregateRoot.Id);
        // remove all events from the aggregate root
        aggregateRoot.ClearEvents();
    }

    public async Task<TAggregate> RehydrateAsync(TKey id,
        CancellationToken cancellationToken = default)
    {
        var pk = new PartitionKey(id.ToString());

        var events = new List<IDomainEvent<TKey, TPrincipalKey>>();

        const string sqlQueryText =
            "SELECT * FROM c WHERE c.type = @type ORDER BY c.version ASC";

        var queryDefinition = new QueryDefinition(sqlQueryText).WithParameter("@type", "EVENT");

        _logger.LogInformation("Rehydrating aggregate root {AggregateRootId}", id);

        var feedIterator =
            _eventsContainer.GetItemQueryStreamIterator(queryDefinition,
                null, new QueryRequestOptions
                {
                    PartitionKey = pk, MaxItemCount = 100
                });

        _logger.LogInformation("Reading events for aggregate root {AggregateRootId}", id);
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
                    document["raisedBy"].ToString(),
                    document["aggregateId"].ToString(), document["aggregateType"].ToString(),
                    document["timestamp"].ToString(), (long)document["version"], (JObject)document["event"])));
        }

        _logger.LogInformation("Got {EventCount} events for aggregate root {AggregateRootId}", events.Count, id);

        var aggregateRoot = _aggregateRootFactory.Create(id, events);
        _logger.LogInformation("Aggregate root {AggregateRootId} rehydrated", id);

        return aggregateRoot;
    }
}