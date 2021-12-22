using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskSetIncomplete), 1.0)]
public class TaskSetIncomplete : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskSetIncomplete(string aggregateType, Guid tenantId, Guid aggregateId, Guid raisedBy, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskSetIncomplete(Task task, Guid raisedBy) : base(task, raisedBy)
    {
    }
}