using System.Collections.Generic;
using System.ComponentModel;
using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate
{
    public interface IAggregateRoot<out TTenantId, out TKey, out TPrincipalId> : IEntity<TTenantId, TKey>
    {
        IReadOnlyCollection<IDomainEvent<TTenantId, TKey, TPrincipalId>> DomainEvents { get; }
        long Version { get; }
        void ClearEvents();
    }
}