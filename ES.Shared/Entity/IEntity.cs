namespace ES.Shared.Entity;

public interface IEntity<out TKey>
{
    TKey Id { get; }
    string ResourceId { get; }
}