namespace ES.Shared.Events;

public interface IDomainEvent<out TKey, out TPrincipalKey>
{
    TKey AggregateId { get; }
    public string AggregateType { get; }
    long Version { get; }
    DateTimeOffset Timestamp { get; }
    TPrincipalKey RaisedBy { get; }
}