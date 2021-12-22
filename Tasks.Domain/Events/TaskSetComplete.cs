using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskSetComplete), 1.0)]
public class TaskSetComplete : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskSetComplete(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskSetComplete(Task task, Guid raisedBy) : base(task, raisedBy)
    {
    }
}