using System.Collections.Generic;
using System.ComponentModel;
using Projects.Shared.Entity;
using Projects.Shared.Events;

namespace Projects.Shared.Aggregate
{
    public interface IAggregateRoot<out TKey> : IEntity<TKey>
    {
        IReadOnlyCollection<IDomainEvent<TKey>> DomainEvents { get; }
        long Version { get; }
        void ClearEvents();
    }
}