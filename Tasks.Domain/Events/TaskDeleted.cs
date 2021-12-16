using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskDeleted), 1.0)]
public class TaskDeleted : BaseDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskDeleted(Task task, Guid raisedBy) : base(task, raisedBy)
    {
    }
}