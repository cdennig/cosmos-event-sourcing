using ES.Shared.Aggregate;
using Newtonsoft.Json;

namespace ES.Shared.Events;

public abstract class DomainEvent<TAggregate, TKey, TPrincipalKey> : IDomainEvent<TKey, TPrincipalKey>
    where TAggregate : IAggregateRoot<TKey, TPrincipalKey>
{
    protected DomainEvent(string aggregateType, TPrincipalKey raisedBy, TKey aggregateId,
        long version, DateTimeOffset timestamp)
    {
        AggregateId = aggregateId;
        AggregateType = aggregateType;
        Version = version;
        Timestamp = timestamp;
        RaisedBy = raisedBy;
    }

    protected DomainEvent(TAggregate aggregateRoot, TPrincipalKey raisedBy)
    {
        if (aggregateRoot is null)
            throw new ArgumentNullException(nameof(aggregateRoot));

        Version = aggregateRoot.Version;
        AggregateId = aggregateRoot.Id;
        AggregateType = typeof(TAggregate).Name.ToLower();
        Timestamp = DateTimeOffset.UtcNow;
        RaisedBy = raisedBy;
    }

    [JsonIgnore] public long Version { get; private set; }
    [JsonIgnore] public TKey AggregateId { get; private set; }
    [JsonIgnore] public string AggregateType { get; private set; }
    [JsonIgnore] public DateTimeOffset Timestamp { get; private set; }
    [JsonIgnore] public TPrincipalKey RaisedBy { get; private set; }
}