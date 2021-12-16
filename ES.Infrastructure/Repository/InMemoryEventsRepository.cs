using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Repository;

namespace ES.Infrastructure.Repository;

public class
    InMemoryEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey> : IEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private Dictionary<string, List<EventData<TTenantKey, TKey, TPrincipalKey>>> _events = new();

    public Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        var domainEvents = aggregateRoot.DomainEvents;
        if (domainEvents.Count == 0) return Task.CompletedTask;

        var key = $"{aggregateRoot.TenantId}:{aggregateRoot.Id}";
        List<EventData<TTenantKey, TKey, TPrincipalKey>> currentEvents;

        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            currentEvents = new List<EventData<TTenantKey, TKey, TPrincipalKey>>();
            _events.Add(key, currentEvents);
        }

        foreach (var domainEvent in domainEvents)
        {
            var ed = new EventData<TTenantKey, TKey, TPrincipalKey>(@domainEvent);
            currentEvents.Add(ed);
        }

        return Task.CompletedTask;
    }

    public Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id, CancellationToken cancellationToken = default)
    {
        var key = $"{tenantId}:{id}";
        List<EventData<TTenantKey, TKey, TPrincipalKey>> currentEvents;
        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            throw new ArgumentException("No events for aggregate.");
        }

        var events = currentEvents.Select(eventData => eventData.Event).ToList();
        var aggregateRoot = BaseAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey>.Create(tenantId, id, events);
        return Task.FromResult(aggregateRoot);
    }
}