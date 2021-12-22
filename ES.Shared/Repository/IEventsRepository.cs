using ES.Shared.Aggregate;

namespace ES.Shared.Repository;

public interface IEventsRepository<TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    Task AppendAsync(TAggregate aggregateRoot, CancellationToken cancellationToken);
    Task<TAggregate> RehydrateAsync(TKey id, CancellationToken cancellationToken = default);
}