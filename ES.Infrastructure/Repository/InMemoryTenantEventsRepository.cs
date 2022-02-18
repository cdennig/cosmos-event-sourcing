using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Repository;

namespace ES.Infrastructure.Repository;

public class
    InMemoryTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantEventsRepository<TTenantKey,
        TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ITenantAggregateRootFactory<TTenantKey,
        TAggregate, TKey, TPrincipalKey> _tenantAggregateRootFactory;

    private Dictionary<string, List<TenantEventData<TTenantKey, TKey, TPrincipalKey>>> _events = new();

    public InMemoryTenantEventsRepository(ITenantAggregateRootFactory<TTenantKey,
        TAggregate, TKey, TPrincipalKey> tenantAggregateRootFactory)
    {
        _tenantAggregateRootFactory = tenantAggregateRootFactory;
    }

    public Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
    {
        var domainEvents = aggregateRoot.DomainEvents;
        if (domainEvents.Count == 0) return Task.CompletedTask;

        var key = $"{aggregateRoot.TenantId}:{aggregateRoot.Id}";
        List<TenantEventData<TTenantKey, TKey, TPrincipalKey>> currentEvents;

        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            currentEvents = new List<TenantEventData<TTenantKey, TKey, TPrincipalKey>>();
            _events.Add(key, currentEvents);
        }

        foreach (var domainEvent in domainEvents)
        {
            var ed = new TenantEventData<TTenantKey, TKey, TPrincipalKey>(@domainEvent);
            currentEvents.Add(ed);
        }

        return Task.CompletedTask;
    }

    public Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id, CancellationToken cancellationToken = default)
    {
        var key = $"{tenantId}:{id}";
        List<TenantEventData<TTenantKey, TKey, TPrincipalKey>> currentEvents;
        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            throw new ArgumentException("No events for aggregate.");
        }

        var events = currentEvents.Select(eventData => eventData.Event).ToList();
        // var aggregateRoot = TenantAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey>.Create(tenantId, id, events);
        var aggregateRoot = _tenantAggregateRootFactory.Create(tenantId, id, events);
        return Task.FromResult(aggregateRoot);
    }
}