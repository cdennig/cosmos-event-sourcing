﻿using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public abstract class BaseAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey> : DomainEntity<TTenantKey, TKey, TPrincipalKey>,
    IAggregateRoot<TTenantKey, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ConcurrentQueue<IDomainEvent<TTenantKey, TKey, TPrincipalKey>> _events = new();

    protected BaseAggregateRoot(TTenantKey tenantId, TKey id) : base(tenantId, id)
    {
    }

    public IReadOnlyCollection<IDomainEvent<TTenantKey, TKey, TPrincipalKey>> DomainEvents => _events.ToImmutableArray();

    public long Version { get; private set; }

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void AddEvent(IDomainEvent<TTenantKey, TKey, TPrincipalKey> @event)
    {
        if (@event.Version != Version)
            throw new ArgumentException("Event applied in wrong order.");

        _events.Enqueue(@event);

        Apply(@event);

        Version++;
    }

    protected abstract void Apply(IDomainEvent<TTenantKey, TKey, TPrincipalKey> @event);

    private static readonly ConstructorInfo ConstructorInfo;

    static BaseAggregateRoot()
    {
        var aggregateType = typeof(TAggregate);
        ConstructorInfo = aggregateType.GetConstructor(
                              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                              null, new[] {typeof(TTenantKey), typeof(TKey)}, Array.Empty<ParameterModifier>()) ??
                          throw new InvalidOperationException();
        if (null == ConstructorInfo)
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
    }

    public static TAggregate Create(TTenantKey tenantId, TKey id, IEnumerable<IDomainEvent<TTenantKey, TKey, TPrincipalKey>> events)
    {
        if (null == tenantId)
            throw new ArgumentNullException(nameof(tenantId));
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));
        var result = (TAggregate) ConstructorInfo.Invoke(new object[] {tenantId, id});

        if (result is BaseAggregateRoot<TTenantKey, TAggregate, TKey, TPrincipalKey> baseAggregate)
            foreach (var @event in events)
                baseAggregate.AddEvent(@event);

        result.ClearEvents();

        return result;
    }
}