namespace Projects.Shared.Entity
{
    public abstract class BaseEntity<TTenantId, TKey> : IEntity<TTenantId, TKey>
    {
        protected BaseEntity()
        {
        }

        protected BaseEntity(TTenantId tenantId, TKey id)
        {
            TenantId = tenantId;
            Id = id;
        }

        public TKey Id { get; protected set; }
        public TTenantId TenantId { get; protected set; }
    }
}