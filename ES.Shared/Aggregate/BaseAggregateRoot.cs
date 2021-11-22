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
    public abstract class BaseAggregateRoot<TTenantId, TA, TKey> : DomainEntity<TTenantId, TKey>,
        IAggregateRoot<TTenantId, TKey>
        where TA : class, IAggregateRoot<TTenantId, TKey>
    {
        private readonly ConcurrentQueue<IDomainEvent<TTenantId, TKey>> _events = new();
        
        protected BaseAggregateRoot(TTenantId tenantId, TKey id) : base(tenantId, id)
        {
        }

        public IReadOnlyCollection<IDomainEvent<TTenantId, TKey>> DomainEvents => _events.ToImmutableArray();

        public long Version { get; private set; }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void AddEvent(IDomainEvent<TTenantId, TKey> @event)
        {
            if (@event.Version != Version)
                throw new ArgumentException("Event applied in wrong order.");

            _events.Enqueue(@event);

            Apply(@event);

            Version++;
        }

        protected abstract void Apply(IDomainEvent<TTenantId, TKey> @event);

        private static readonly ConstructorInfo ConstructorInfo;

        static BaseAggregateRoot()
        {
            var aggregateType = typeof(TA);
            ConstructorInfo = aggregateType.GetConstructor(
                                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                  null, new[] { typeof(TTenantId), typeof(TKey) }, Array.Empty<ParameterModifier>()) ??
                              throw new InvalidOperationException();
            if (null == ConstructorInfo)
                throw new InvalidOperationException(
                    $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
        }

        public static TA Create(TTenantId tenantId, TKey id, IEnumerable<IDomainEvent<TTenantId, TKey>> events)
        {
            if (null == tenantId)
                throw new ArgumentNullException(nameof(tenantId));
            if (null == id)
                throw new ArgumentNullException(nameof(id));
            if (null == events || !events.Any())
                throw new ArgumentNullException(nameof(events));
            var result = (TA)ConstructorInfo.Invoke(new object[] { tenantId, id });

            if (result is BaseAggregateRoot<TTenantId, TA, TKey> baseAggregate)
                foreach (var @event in events)
                    baseAggregate.AddEvent(@event);

            result.ClearEvents();

            return result;
        }
    }
}