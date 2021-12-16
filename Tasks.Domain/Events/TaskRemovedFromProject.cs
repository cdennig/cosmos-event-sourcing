using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskRemovedFromProject), 1.0)]
public class TaskRemovedFromProject : BaseDomainEvent<Guid, Task, Guid, Guid>
{
    public TaskRemovedFromProject(Task task, Guid raisedBy) : base(task, raisedBy)
    {
        OldProject = task.ProjectId;
    }

    private TaskRemovedFromProject(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid? OldProject { get; set; }
}