namespace ES.Shared.Events;

public interface IDomainEvent<out TTenantKey, out TKey, out TPrincipalKey>
{
    TTenantKey TenantId { get; }
    TKey AggregateId { get; }
    public string AggregateType { get; }
    long Version { get; }
    DateTimeOffset Timestamp { get; }
    TPrincipalKey RaisedBy { get; }
}