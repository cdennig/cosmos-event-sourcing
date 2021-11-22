namespace ES.Shared.Entity
{
    public interface IEntity<out TTenantId, out TKey>
    {
        TTenantId TenantId { get; }
        TKey Id { get; }
        string ResourceId { get; }
    }
}