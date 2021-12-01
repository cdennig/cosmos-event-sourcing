using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskDescriptionsUpdated : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        public TaskDescriptionsUpdated(Task task, Guid raisedBy, string title, string description) : base(task,
            raisedBy)
        {
            Title = title;
            Description = description;
        }

        private TaskDescriptionsUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
            long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public string? Description { get; set; }
        [JsonProperty] public string? Title { get; set; }
    }
}