using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Repository;
using Microsoft.Extensions.Logging;

namespace ES.Infrastructure.Repository;

public class
    InMemoryTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantEventsRepository<TTenantKey,
        TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ILogger<InMemoryTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>> _logger;

    private readonly ITenantAggregateRootFactory<TTenantKey,
        TAggregate, TKey, TPrincipalKey> _tenantAggregateRootFactory;

    private readonly Dictionary<string, List<TenantEventData<TTenantKey, TKey, TPrincipalKey>>> _events = new();

    public InMemoryTenantEventsRepository(ITenantAggregateRootFactory<TTenantKey,
            TAggregate, TKey, TPrincipalKey> tenantAggregateRootFactory,
        ILogger<InMemoryTenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>> logger)
    {
        _tenantAggregateRootFactory = tenantAggregateRootFactory;
        _logger = logger;
    }

    public Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken = default)
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
            return Task.CompletedTask;
        }

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

        _logger.LogInformation(
            "Successfully appended domain events of aggregate root {AggregateRootId} / tenant {TenantId}. Clearing events now",
            aggregateRoot.Id, aggregateRoot.TenantId);

        // remove all events from the aggregate root
        aggregateRoot.ClearEvents();

        return Task.CompletedTask;
    }

    public Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rehydrating aggregate root {AggregateRootId} / tenant {TenantId}", id, tenantId);
        var key = $"{tenantId}:{id}";
        List<TenantEventData<TTenantKey, TKey, TPrincipalKey>> currentEvents;
        if (_events.ContainsKey(key))
            currentEvents = _events[key];
        else
        {
            throw new ArgumentException("No events for aggregate.");
        }

        _logger.LogInformation("Reading events for aggregate root {AggregateRootId} / tenant {TenantId}", id, tenantId);
        var events = currentEvents.Select(eventData => eventData.Event).ToList();

        _logger.LogInformation("Got {EventCount} events for aggregate root {AggregateRootId} / tenant {TenantId}", events.Count, id, tenantId);
        var aggregateRoot = _tenantAggregateRootFactory.Create(tenantId, id, events);

        _logger.LogInformation("Aggregate root {AggregateRootId} / tenant {TenantId} rehydrated", id, tenantId);
        return Task.FromResult(aggregateRoot);
    }
}