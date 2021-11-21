using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Projects.Shared.Entity;
using Projects.Shared.Events;

namespace Projects.Shared.Aggregate
{
    public abstract class BaseAggregateRoot<TA, TKey> : DomainEntity<TKey>, IAggregateRoot<TKey>
        where TA : class, IAggregateRoot<TKey>
    {
        private readonly ConcurrentQueue<IDomainEvent<TKey>> _events = new();

        protected BaseAggregateRoot()
        {
        }

        protected BaseAggregateRoot(TKey id) : base(id)
        {
        }

        public IReadOnlyCollection<IDomainEvent<TKey>> DomainEvents => _events.ToImmutableArray();

        public long Version { get; private set; }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void AddEvent(IDomainEvent<TKey> @event)
        {
            if (@event.Version != Version)
                throw new ArgumentException("Event applied in wrong order.");

            _events.Enqueue(@event);

            Apply(@event);

            Version++;
        }

        protected abstract void Apply(IDomainEvent<TKey> @event);

        private static readonly ConstructorInfo ConstructorInfo;

        static BaseAggregateRoot()
        {
            var aggregateType = typeof(TA);
            ConstructorInfo = aggregateType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, new Type[0], new ParameterModifier[0]);
            if (null == ConstructorInfo)
                throw new InvalidOperationException(
                    $"Unable to find required private parameterless constructor for Aggregate of type '{aggregateType.Name}'");
        }

        public static TA Create(IEnumerable<IDomainEvent<TKey>> events)
        {
            if (null == events || !events.Any())
                throw new ArgumentNullException(nameof(events));
            var result = (TA) ConstructorInfo.Invoke(Array.Empty<object>());

            if (result is BaseAggregateRoot<TA, TKey> baseAggregate)
                foreach (var @event in events)
                    baseAggregate.AddEvent(@event);

            result.ClearEvents();

            return result;
        }
    }
}