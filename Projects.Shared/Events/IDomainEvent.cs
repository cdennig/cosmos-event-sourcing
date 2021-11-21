using System;

namespace Projects.Shared.Events
{
    public interface IDomainEvent<out TKey>
    {
        TKey AggregateId { get; }
        public string AggregateType { get; }
        long Version { get; }
        DateTimeOffset Timestamp { get; }
    }
}