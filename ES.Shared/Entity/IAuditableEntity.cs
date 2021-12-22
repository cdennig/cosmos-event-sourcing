namespace ES.Shared.Entity;

public interface IAuditableEntity<out TPrincipalKey>
{
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ModifiedAt { get; }
    public DateTimeOffset? DeletedAt { get; }
        
    public TPrincipalKey CreatedBy { get; }
    public TPrincipalKey? ModifiedBy { get; }
    public TPrincipalKey? DeletedBy { get; }
}