namespace ES.Shared.Entity
{
    public abstract class BaseEntity<TTenantId, TKey> : IEntity<TTenantId, TKey>
    {

        protected BaseEntity(TTenantId tenantId, TKey id)
        {
            TenantId = tenantId;
            Id = id;
        }

        public TKey Id { get; }
        public TTenantId TenantId { get; }
        public abstract string ResourceId { get; }
    }
}