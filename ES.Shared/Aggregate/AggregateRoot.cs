using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public abstract class AggregateRoot<TAggregate, TKey, TPrincipalKey> :
    AuditableEntity<TKey, TPrincipalKey>, IAggregateRoot<TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly ConcurrentQueue<IDomainEvent<TKey, TPrincipalKey>> _events = new();
    private readonly object _updateEventsLock = new object();

    protected AggregateRoot(TKey id) : base(id)
    {
    }

    protected AggregateRoot(TKey id,
        IEnumerable<IDomainEvent<TKey, TPrincipalKey>> @events) : base(id)
    {
        BatchAddEvents(@events);
    }

    public IReadOnlyCollection<IDomainEvent<TKey, TPrincipalKey>> DomainEvents =>
        _events.ToImmutableArray();

    public long Version { get; private set; }

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void AddEvent(IDomainEvent<TKey, TPrincipalKey> @event)
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

    private void BatchAddEvents(IEnumerable<IDomainEvent<TKey, TPrincipalKey>> @events)
    {
        var domainEvents = @events.ToList();
        if (domainEvents.First().Version != Version)
            throw new ArgumentException("Events applied in wrong order.");

        if (Monitor.TryEnter(_updateEventsLock, 300))
        {
            try
            {
                foreach (var @event in domainEvents)
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

    protected abstract void Apply(IDomainEvent<TKey, TPrincipalKey> @event);
}