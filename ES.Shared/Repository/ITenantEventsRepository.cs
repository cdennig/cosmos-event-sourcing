using ES.Shared.Aggregate;

namespace ES.Shared.Repository;

public interface ITenantEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken);
    Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id, CancellationToken cancellationToken = default);
}