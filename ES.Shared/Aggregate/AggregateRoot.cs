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

    protected AggregateRoot(TKey id) : base(id)
    {
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

        _events.Enqueue(@event);

        Apply(@event);

        Version++;
    }

    protected abstract void Apply(IDomainEvent<TKey, TPrincipalKey> @event);

    private static readonly ConstructorInfo ConstructorInfo;

    static AggregateRoot()
    {
        var aggregateType = typeof(TAggregate);
        ConstructorInfo = aggregateType.GetConstructor(
                              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                              null, new[] {typeof(TKey)}, Array.Empty<ParameterModifier>()) ??
                          throw new InvalidOperationException();
        if (null == ConstructorInfo)
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
    }

    public static TAggregate Create(TKey id,
        IEnumerable<IDomainEvent<TKey, TPrincipalKey>> events)
    {
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));
        var result = (TAggregate) ConstructorInfo.Invoke(new object[] {id});

        if (result is AggregateRoot<TAggregate, TKey, TPrincipalKey> baseAggregate)
            foreach (var @event in events)
                baseAggregate.AddEvent(@event);

        result.ClearEvents();

        return result;
    }
}