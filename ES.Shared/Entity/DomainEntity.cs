namespace ES.Shared.Entity;

public abstract class DomainEntity<TTenantKey, TKey, TPrincipalKey> : BaseEntity<TTenantKey, TKey>
{
    protected DomainEntity(TTenantKey tenantId, TKey id) : base(tenantId, id)
    {
    }

    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
        
    public TPrincipalKey CreatedBy { get; protected set; }
    public TPrincipalKey? ModifiedBy { get; protected set; }
    public TPrincipalKey? DeletedBy { get; protected set; }

    public bool Deleted { get; protected set; }
}