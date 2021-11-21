using System;

namespace Projects.Shared.Entity
{
    public abstract class DomainEntity<TKey> : BaseEntity<TKey>
    {
        protected DomainEntity() { }

        protected DomainEntity(TKey id) : base(id) { }

        public DateTimeOffset CreatedAt { get; protected set; }
        public DateTimeOffset? ModifiedAt { get; protected set; }
        public DateTimeOffset? DeletedAt { get; protected set; }

        public bool Deleted { get; protected set; }
    }
}