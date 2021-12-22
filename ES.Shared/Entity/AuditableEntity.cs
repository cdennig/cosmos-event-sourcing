namespace ES.Shared.Entity;

public abstract class AuditableEntity<TKey, TPrincipalKey> : Entity<TKey>,
    IAuditableEntity<TPrincipalKey>
{
    protected AuditableEntity(TKey id) : base(id)
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