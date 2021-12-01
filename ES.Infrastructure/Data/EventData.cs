﻿using System;
using ES.Shared.Events;

namespace ES.Infrastructure.Data
{
    public class EventData<TTenantId, TKey, TPrincipalId>
    {
        public Guid Id { get; }
        public TTenantId TenantId { get; }
        public string PartitionKey { get; }
        public string Type => "EVENT";
        public string? EventType { get; }
        public TKey AggregateId { get; }
        public TPrincipalId RaisedBy { get; }
        public string AggregateType { get; }
        public long Version { get; }
        public DateTimeOffset Timestamp { get; }
        public IDomainEvent<TTenantId, TKey, TPrincipalId> Event { get; }

        public EventData(IDomainEvent<TTenantId, TKey, TPrincipalId> @event)
        {
            Id = Guid.NewGuid();
            TenantId = @event.TenantId;
            PartitionKey = @event.AggregateId.ToString();
            EventType = @event.GetType().FullName;
            AggregateId = @event.AggregateId;
            AggregateType = @event.AggregateType;
            Version = @event.Version;
            Timestamp = @event.Timestamp;
            RaisedBy = @event.RaisedBy;
            Event = @event;
        }
    }
}