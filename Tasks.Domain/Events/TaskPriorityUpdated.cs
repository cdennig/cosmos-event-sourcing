using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskPriorityUpdated : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        public TaskPriorityUpdated(Task task, Guid raisedBy, TaskPriority priority) : base(task, raisedBy)
        {
            NewPriority = priority;
            OldPriority = task.Priority;
        }

        private TaskPriorityUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public TaskPriority? NewPriority { get; set; }
        [JsonProperty] public TaskPriority? OldPriority { get; set; }
    }
}