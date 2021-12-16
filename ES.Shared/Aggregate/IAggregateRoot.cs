using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public interface IAggregateRoot<out TTenantKey, out TKey, out TPrincipalKey> : IEntity<TTenantKey, TKey>
{
    IReadOnlyCollection<IDomainEvent<TTenantKey, TKey, TPrincipalKey>> DomainEvents { get; }
    long Version { get; }
    void ClearEvents();
}