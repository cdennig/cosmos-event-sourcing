using System.Collections.Generic;
using System.ComponentModel;
using Projects.Shared.Entity;
using Projects.Shared.Events;

namespace Projects.Shared.Aggregate
{
    public interface IAggregateRoot<out TTenantId, out TKey> : IEntity<TTenantId, TKey>
    {
        TTenantId TenantId { get; }
        IReadOnlyCollection<IDomainEvent<TTenantId, TKey>> DomainEvents { get; }
        long Version { get; }
        void ClearEvents();
    }
}