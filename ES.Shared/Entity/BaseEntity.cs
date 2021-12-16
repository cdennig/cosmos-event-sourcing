namespace ES.Shared.Entity;

public abstract class BaseEntity<TTenantKey, TKey> : IEntity<TTenantKey, TKey>
{

    protected BaseEntity(TTenantKey tenantId, TKey id)
    {
        TenantId = tenantId;
        Id = id;
    }

    public TKey Id { get; }
    public TTenantKey TenantId { get; }
    public abstract string ResourceId { get; }
}