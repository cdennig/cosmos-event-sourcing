using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public interface ITenantAggregateRootFactory<in TTenantKey, out TAggregate, in TKey, in TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    public TAggregate Create(TTenantKey tenantId, TKey id,
        IEnumerable<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> events);
}