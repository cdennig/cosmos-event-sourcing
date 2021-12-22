using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskUndeleted), 1.0)]
public class TaskUndeleted : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskUndeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskUndeleted(Task task, Guid raisedBy) : base(task, raisedBy)
    {
    }
}