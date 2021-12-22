namespace ES.Shared.Events;

public interface ITenantDomainEvent<out TTenantKey, out TKey, out TPrincipalKey> : IDomainEvent<TKey, TPrincipalKey>
{
    TTenantKey TenantId { get; }
}