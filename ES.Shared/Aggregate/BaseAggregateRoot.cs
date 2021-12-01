using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate
{
    public abstract class BaseAggregateRoot<TTenantId, TA, TKey, TPrincipalId> : DomainEntity<TTenantId, TKey, TPrincipalId>,
        IAggregateRoot<TTenantId, TKey, TPrincipalId>
        where TA : class, IAggregateRoot<TTenantId, TKey, TPrincipalId>
    {
        private readonly ConcurrentQueue<IDomainEvent<TTenantId, TKey, TPrincipalId>> _events = new();

        protected BaseAggregateRoot(TTenantId tenantId, TKey id) : base(tenantId, id)
        {
        }

        public IReadOnlyCollection<IDomainEvent<TTenantId, TKey, TPrincipalId>> DomainEvents => _events.ToImmutableArray();

        public long Version { get; private set; }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void AddEvent(IDomainEvent<TTenantId, TKey, TPrincipalId> @event)
        {
            if (@event.Version != Version)
                throw new ArgumentException("Event applied in wrong order.");

            _events.Enqueue(@event);

            Apply(@event);

            Version++;
        }

        protected abstract void Apply(IDomainEvent<TTenantId, TKey, TPrincipalId> @event);

        private static readonly ConstructorInfo ConstructorInfo;

        static BaseAggregateRoot()
        {
            var aggregateType = typeof(TA);
            ConstructorInfo = aggregateType.GetConstructor(
                                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                  null, new[] {typeof(TTenantId), typeof(TKey)}, Array.Empty<ParameterModifier>()) ??
                              throw new InvalidOperationException();
            if (null == ConstructorInfo)
                throw new InvalidOperationException(
                    $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
        }

        public static TA Create(TTenantId tenantId, TKey id, IEnumerable<IDomainEvent<TTenantId, TKey, TPrincipalId>> events)
        {
            if (null == tenantId)
                throw new ArgumentNullException(nameof(tenantId));
            if (null == id)
                throw new ArgumentNullException(nameof(id));
            if (null == events || !events.Any())
                throw new ArgumentNullException(nameof(events));
            var result = (TA) ConstructorInfo.Invoke(new object[] {tenantId, id});

            if (result is BaseAggregateRoot<TTenantId, TA, TKey, TPrincipalId> baseAggregate)
                foreach (var @event in events)
                    baseAggregate.AddEvent(@event);

            result.ClearEvents();

            return result;
        }
    }
}