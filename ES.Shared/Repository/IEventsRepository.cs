using ES.Shared.Aggregate;

namespace ES.Shared.Repository;

public interface IEventsRepository<TTenantKey, TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken);
    Task<TAggregate> RehydrateAsync(TTenantKey tenantId, TKey id, CancellationToken cancellationToken = default);
}