using System;

namespace Projects.Shared.Events
{
    public interface IDomainEvent<out TTenantId, out TKey>
    {
        TTenantId TenantId { get; }
        TKey AggregateId { get; }
        public string AggregateType { get; }
        long Version { get; }
        DateTimeOffset Timestamp { get; }
    }
}