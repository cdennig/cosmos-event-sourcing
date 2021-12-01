using System;

namespace ES.Shared.Events
{
    public interface IDomainEvent<out TTenantId, out TKey, out TPrincipalId>
    {
        TTenantId TenantId { get; }
        TKey AggregateId { get; }
        public string AggregateType { get; }
        long Version { get; }
        DateTimeOffset Timestamp { get; }
        TPrincipalId RaisedBy { get; }
    }
}