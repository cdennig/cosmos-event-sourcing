using System;
using ES.Shared.Aggregate;
using Newtonsoft.Json;

namespace ES.Shared.Events
{
    public abstract class BaseDomainEvent<TTenantId, TA, TKey> : IDomainEvent<TTenantId, TKey>
        where TA : IAggregateRoot<TTenantId, TKey>
    {
        protected BaseDomainEvent(string aggregateType, TTenantId tenantId, TKey aggregateId,
            long version, DateTimeOffset timestamp)
        {
            TenantId = tenantId;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            Version = version;
            Timestamp = timestamp;
        }

        protected BaseDomainEvent(TA aggregateRoot)
        {
            if (aggregateRoot is null)
                throw new ArgumentNullException(nameof(aggregateRoot));

            TenantId = aggregateRoot.TenantId;
            Version = aggregateRoot.Version;
            AggregateId = aggregateRoot.Id;
            AggregateType = typeof(TA).Name.ToLower();
            Timestamp = DateTimeOffset.UtcNow;
        }

        [JsonIgnore] public long Version { get; private set; }
        [JsonIgnore] public TTenantId TenantId { get; private set; }
        [JsonIgnore] public TKey AggregateId { get; private set; }
        [JsonIgnore] public string AggregateType { get; private set; }
        [JsonIgnore] public DateTimeOffset Timestamp { get; private set; }
    }
}