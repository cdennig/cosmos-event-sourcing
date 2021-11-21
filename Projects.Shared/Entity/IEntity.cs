namespace Projects.Shared.Entity
{
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}