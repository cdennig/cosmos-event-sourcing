using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskAssignedToProject : BaseDomainEvent<Guid, Task, Guid>
    {
        public TaskAssignedToProject(Task task, Guid projectId) : base(task)
        {
            NewProject = projectId;
            OldProject = task.ProjectId;
        }

        private TaskAssignedToProject(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public Guid NewProject { get; set; }
        [JsonProperty] public Guid? OldProject { get; set; }
    }
}