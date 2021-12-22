namespace ES.Shared.Entity;

public abstract class Entity<TKey> : IEntity<TKey>
{
    protected Entity(TKey id)
    {
        Id = id;
    }

    public TKey Id { get; }
    public abstract string ResourceId { get; }
}