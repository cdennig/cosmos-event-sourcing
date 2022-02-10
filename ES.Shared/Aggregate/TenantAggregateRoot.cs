using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public abstract class TenantAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey> :
    AuditableTenantEntity<TTenantKey, TKey, TPrincipalKey>,
    ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ConcurrentQueue<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> _events = new();
    private readonly object _updateEventsLock = new object();

    protected TenantAggregateRoot(TTenantKey tenantId, TKey id) : base(tenantId, id)
    {
    }

    public IReadOnlyCollection<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> DomainEvents =>
        _events.ToImmutableArray();

    public long Version { get; private set; }

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void AddEvent(ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> @event)
    {
        if (@event.Version != Version)
            throw new ArgumentException("Event applied in wrong order.");

        if (Monitor.TryEnter(_updateEventsLock, 300))
        {
            try
            {
                _events.Enqueue(@event);

                Apply(@event);
                Version++;
            }
            finally
            {
                Monitor.Exit(_updateEventsLock);
            }
        }
        else
        {
            throw new Exception($"Could not acquire lock on aggregate ID {Id} / Version {Version}");
        }
    }

    private void BatchAddEvents(IEnumerable<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> @events)
    {
        var tenantDomainEvents = @events.ToList();
        if (tenantDomainEvents.First().Version != Version)
            throw new ArgumentException("Events applied in wrong order.");

        if (Monitor.TryEnter(_updateEventsLock, 300))
        {
            try
            {
                foreach (var @event in tenantDomainEvents)
                {
                    _events.Enqueue(@event);

                    Apply(@event);
                    Version++;
                }
            }
            finally
            {
                Monitor.Exit(_updateEventsLock);
            }
        }
        else
        {
            throw new Exception($"Could not acquire lock on aggregate ID {Id} / Version {Version}");
        }
    }

    protected abstract void Apply(ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> @event);

    private static readonly ConstructorInfo ConstructorInfo;

    static TenantAggregateRoot()
    {
        var aggregateType = typeof(TAggregate);
        ConstructorInfo = aggregateType.GetConstructor(
                              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                              null, new[] { typeof(TTenantKey), typeof(TKey) }, Array.Empty<ParameterModifier>()) ??
                          throw new InvalidOperationException();
        if (null == ConstructorInfo)
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
    }

    public static TAggregate Create(TTenantKey tenantId, TKey id,
        IEnumerable<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> events)
    {
        if (null == tenantId)
            throw new ArgumentNullException(nameof(tenantId));
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));
        var result = (TAggregate)ConstructorInfo.Invoke(new object[] { tenantId, id });

        if (result is TenantAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey> baseAggregate)
            baseAggregate.BatchAddEvents(events);

        result.ClearEvents();

        return result;
    }
}