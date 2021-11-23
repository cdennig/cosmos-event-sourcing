using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskPriorityUpdated : BaseDomainEvent<Guid, Task, Guid>
    {
        public TaskPriorityUpdated(Task task, TaskPriority priority) : base(task)
        {
            NewPriority = priority;
            OldPriority = task.Priority;
        }

        private TaskPriorityUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public TaskPriority NewPriority { get; set; }
        [JsonProperty] public TaskPriority OldPriority { get; set; }
    }
}