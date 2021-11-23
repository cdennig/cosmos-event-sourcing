using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskDescriptionsUpdated : BaseDomainEvent<Guid, Task, Guid>
    {
        public TaskDescriptionsUpdated(Task task, string title, string description) : base(task)
        {
            Title = title;
            Description = description;
        }

        private TaskDescriptionsUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public string Title { get; set; }
    }
}