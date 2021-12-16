using ES.Shared.Aggregate;
using Newtonsoft.Json;

namespace ES.Shared.Events;

public abstract class BaseDomainEvent<TTenantKey, TAggregate, TKey, TPrincipalKey> : IDomainEvent<TTenantKey, TKey, TPrincipalKey>
    where TAggregate : IAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    protected BaseDomainEvent(string aggregateType, TTenantKey tenantId, TPrincipalKey raisedBy, TKey aggregateId,
        long version, DateTimeOffset timestamp)
    {
        TenantId = tenantId;
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        Version = version;
        Timestamp = timestamp;
        RaisedBy = raisedBy;
    }

    protected BaseDomainEvent(TAggregate aggregateRoot, TPrincipalKey raisedBy)
    {
        if (aggregateRoot is null)
            throw new ArgumentNullException(nameof(aggregateRoot));

        TenantId = aggregateRoot.TenantId;
        Version = aggregateRoot.Version;
        AggregateId = aggregateRoot.Id;
        AggregateType = typeof(TAggregate).Name.ToLower();
        Timestamp = DateTimeOffset.UtcNow;
        RaisedBy = raisedBy;
    }

    [JsonIgnore] public long Version { get; private set; }
    [JsonIgnore] public TTenantKey TenantId { get; private set; }
    [JsonIgnore] public TKey AggregateId { get; private set; }
    [JsonIgnore] public string AggregateType { get; private set; }
    [JsonIgnore] public DateTimeOffset Timestamp { get; private set; }
    [JsonIgnore] public TPrincipalKey RaisedBy { get; private set; }
}