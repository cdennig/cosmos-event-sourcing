using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public interface IAggregateRootFactory<out TAggregate, in TKey, in TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    public TAggregate Create(TKey id,
        IEnumerable<IDomainEvent<TKey, TPrincipalKey>> events);
}