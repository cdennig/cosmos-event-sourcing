using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public interface ITenantAggregateRoot<out TTenantKey, out TKey, out TPrincipalKey> : ITenantEntity<TTenantKey, TKey>
{
    IReadOnlyCollection<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> DomainEvents { get; }
    long Version { get; }
    void ClearEvents();
}