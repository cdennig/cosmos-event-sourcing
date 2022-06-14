using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Repository;
using Microsoft.Extensions.Logging;

namespace ES.Infrastructure.Repository;

public class
    InMemoryEventsRepository<TAggregate, TKey, TPrincipalKey> : IEventsRepository<TAggregate, TKey,
        TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly ILogger<InMemoryEventsRepository<TAggregate, TKey, TPrincipalKey>> _logger;
    private readonly IAggregateRootFactory<TAggregate, TKey, TPrincipalKey> _aggregateRootFactory;
    private readonly Dictionary<string, List<EventData<TKey, TPrincipalKey>>> _events = new();

    public InMemoryEventsRepository(IAggregateRootFactory<TAggregate, TKey, TPrincipalKey> aggregateRootFactory,
        ILogger<InMemoryEventsRepository<TAggregate, TKey, TPrincipalKey>> logger)
    {
        _aggregateRootFactory = aggregateRootFactory;
        _logger = logger;
    }

    public Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Appending {EventCount} domain events of aggregate root {AggregateRootId}",
            aggregateRoot.DomainEvents.Count, aggregateRoot.Id);
        var domainEvents = aggregateRoot.DomainEvents;
        if (domainEvents.Count == 0)
        {
            _logger.LogWarning(
                "Tried to append domain events of aggregate root {AggregateRootId}, but DomainEvents was empty",
                aggregateRoot.Id);
            return Task.CompletedTask;
        }


        var key = $"{aggregateRoot.Id}";
        List<EventData<TKey, TPrincipalKey>> currentEvents;

        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            currentEvents = new List<EventData<TKey, TPrincipalKey>>();
            _events.Add(key, currentEvents);
        }

        foreach (var domainEvent in domainEvents)
        {
            var ed = new EventData<TKey, TPrincipalKey>(@domainEvent);
            currentEvents.Add(ed);
        }

        _logger.LogInformation(
            "Successfully appended domain events of aggregate root {AggregateRootId}. Clearing events now",
            aggregateRoot.Id);

        // remove all events from the aggregate root
        aggregateRoot.ClearEvents();

        return Task.CompletedTask;
    }

    public Task<TAggregate> RehydrateAsync(TKey id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rehydrating aggregate root {AggregateRootId}", id);
        var key = $"{id}";
        List<EventData<TKey, TPrincipalKey>> currentEvents;
        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            throw new ArgumentException("No events for aggregate.");
        }
        
        _logger.LogInformation("Reading events for aggregate root {AggregateRootId}", id);
        var events = currentEvents.Select(eventData => eventData.Event).ToList();
        
        _logger.LogInformation("Got {EventCount} events for aggregate root {AggregateRootId}", events.Count, id);
        var aggregateRoot = _aggregateRootFactory.Create(id, events);
        
        _logger.LogInformation("Aggregate root {AggregateRootId} rehydrated", id);
        return Task.FromResult(aggregateRoot);
    }
}