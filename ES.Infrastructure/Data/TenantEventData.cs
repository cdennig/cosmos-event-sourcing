﻿using ES.Shared.Attributes;
using ES.Shared.Events;

namespace ES.Infrastructure.Data;

public class TenantEventData<TTenantKey, TKey, TPrincipalKey>
{
    public Guid Id { get; }
    public TTenantKey TenantId { get; }
    public string PartitionKey { get; }
    public string Type => "EVENT";
    public string? EventType { get; }
    public double EventVersion { get; }
    public TKey AggregateId { get; }
    public TPrincipalKey RaisedBy { get; }
    public string AggregateType { get; }
    public long Version { get; }
    public DateTimeOffset Timestamp { get; }
    public ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> Event { get; }

    public TenantEventData(ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> @event)
    {
        Id = Guid.NewGuid();
        TenantId = @event.TenantId;
        PartitionKey = @event.AggregateId.ToString();

        // Using reflection for event attribute
        var attrs = Attribute.GetCustomAttributes(@event.GetType());
        foreach (var attr in attrs)
        {
            if (attr is EventAttribute attribute)
            {
                EventType = attribute.EventType;
                EventVersion = attribute.EventVersion;
            }
        }

        AggregateId = @event.AggregateId;
        AggregateType = @event.AggregateType;
        Version = @event.Version;
        Timestamp = @event.Timestamp;
        RaisedBy = @event.RaisedBy;
        Event = @event;
    }
}