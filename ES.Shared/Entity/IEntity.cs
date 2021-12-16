namespace ES.Shared.Entity;

public interface IEntity<out TTenantKey, out TKey>
{
    TTenantKey TenantId { get; }
    TKey Id { get; }
    string ResourceId { get; }
}