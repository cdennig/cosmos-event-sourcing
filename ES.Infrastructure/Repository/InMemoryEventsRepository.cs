using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Repository;

namespace ES.Infrastructure.Repository;

public class
    InMemoryEventsRepository<TAggregate, TKey, TPrincipalKey> : IEventsRepository<TAggregate, TKey,
        TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private Dictionary<string, List<EventData<TKey, TPrincipalKey>>> _events = new();

    public Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        var domainEvents = aggregateRoot.DomainEvents;
        if (domainEvents.Count == 0) return Task.CompletedTask;

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

        return Task.CompletedTask;
    }

    public Task<TAggregate> RehydrateAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var key = $"{id}";
        List<EventData<TKey, TPrincipalKey>> currentEvents;
        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            throw new ArgumentException("No events for aggregate.");
        }

        var events = currentEvents.Select(eventData => eventData.Event).ToList();
        var aggregateRoot =
            AggregateRoot<TAggregate, TKey, TPrincipalKey>.Create(id, events);
        return Task.FromResult(aggregateRoot);
    }
}