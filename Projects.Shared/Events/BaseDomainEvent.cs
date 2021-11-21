using System;
using Newtonsoft.Json;
using Projects.Shared.Aggregate;

namespace Projects.Shared.Events
{
    public abstract class BaseDomainEvent<TA, TKey> : IDomainEvent<TKey>
        where TA : IAggregateRoot<TKey>
    {
        protected BaseDomainEvent(string aggregateType, TKey aggregateId,
            long version, DateTimeOffset timestamp)
        {
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            Version = version;
            Timestamp = timestamp;
        }

        protected BaseDomainEvent(TA aggregateRoot)
        {
            if (aggregateRoot is null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            Version = aggregateRoot.Version;
            AggregateId = aggregateRoot.Id;
            AggregateType = typeof(TA).Name.ToLower();
            Timestamp = DateTimeOffset.UtcNow;
        }

        [JsonIgnore] public long Version { get; private set; }
        [JsonIgnore] public TKey AggregateId { get; private set; }
        [JsonIgnore] public string AggregateType { get; private set; }
        [JsonIgnore] public DateTimeOffset Timestamp { get; private set; }
    }
}