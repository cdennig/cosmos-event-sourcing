using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskAssignedToProject), 1.0)]
public class TaskAssignedToProject : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    public TaskAssignedToProject(Task task, Guid raisedBy, Guid projectId) : base(task, raisedBy)
    {
        NewProject = projectId;
        OldProject = task.ProjectId;
    }

    private TaskAssignedToProject(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid NewProject { get; set; }
    [JsonProperty] public Guid? OldProject { get; set; }
}