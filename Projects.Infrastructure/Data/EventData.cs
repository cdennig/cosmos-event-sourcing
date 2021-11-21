﻿using System;
using Projects.Shared.Events;

namespace Projects.Infrastructure.Data
{
    public class EventData<TTenantId, TKey>
    {
        public Guid Id { get; }
        public TTenantId TenantId { get; }
        public string PartitionKey { get; }
        public string Type { get; } = "EVENT";
        public string EventType { get; }
        public TKey AggregateId { get; }
        public string AggregateType { get; }
        public long Version { get; }
        public DateTimeOffset Timestamp { get; }
        public IDomainEvent<TTenantId, TKey> @Event { get; }

        public EventData(IDomainEvent<TTenantId, TKey> @event)
        {
            Id = Guid.NewGuid();
            PartitionKey = @event.AggregateId.ToString();
            EventType = @event.GetType().FullName;
            AggregateId = @event.AggregateId;
            AggregateType = @event.AggregateType;
            Version = @event.Version;
            Timestamp = @event.Timestamp;
            Event = @event;
        }
    }
}