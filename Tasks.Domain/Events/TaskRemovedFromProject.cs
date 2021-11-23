using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskRemovedFromProject : BaseDomainEvent<Guid, Task, Guid>
    {
        public TaskRemovedFromProject(Task task) : base(task)
        {
            OldProject = task.ProjectId;
        }

        private TaskRemovedFromProject(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }
        
        [JsonProperty] public Guid? OldProject { get; set; }
    }
}