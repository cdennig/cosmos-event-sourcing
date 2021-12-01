using System;

namespace ES.Shared.Entity
{
    public abstract class DomainEntity<TTenantId, TKey, TPrincipalId> : BaseEntity<TTenantId, TKey>
    {
        protected DomainEntity(TTenantId tenantId, TKey id) : base(tenantId, id)
        {
        }

        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? ModifiedAt { get; protected set; }
        public DateTimeOffset? DeletedAt { get; protected set; }
        
        public TPrincipalId CreatedBy { get; protected set; }
        public TPrincipalId? ModifiedBy { get; protected set; }
        public TPrincipalId? DeletedBy { get; protected set; }

        public bool Deleted { get; protected set; }
    }
}