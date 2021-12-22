using ES.Shared.Entity;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public interface IAggregateRoot<out TKey, out TPrincipalKey> : IEntity<TKey>
{
    IReadOnlyCollection<IDomainEvent<TKey, TPrincipalKey>> DomainEvents { get; }
    long Version { get; }
    void ClearEvents();
}