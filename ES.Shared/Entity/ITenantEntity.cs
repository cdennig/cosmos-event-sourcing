namespace ES.Shared.Entity;

public interface ITenantEntity<out TTenantKey, out TKey> : IEntity<TKey>
{
    TTenantKey TenantId { get; }
}