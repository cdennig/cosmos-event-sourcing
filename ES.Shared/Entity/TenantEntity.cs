namespace ES.Shared.Entity;

public abstract class TenantEntity<TTenantKey, TKey> : ITenantEntity<TTenantKey, TKey>
{

    protected TenantEntity(TTenantKey tenantId, TKey id)
    {
        TenantId = tenantId;
        Id = id;
    }

    public TKey Id { get; }
    public TTenantKey TenantId { get; }
    public abstract string ResourceId { get; }
}