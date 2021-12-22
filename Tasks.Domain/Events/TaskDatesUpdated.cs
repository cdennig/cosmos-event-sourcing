using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskDatesUpdated), 1.0)]
public class TaskDatesUpdated : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskDatesUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskDatesUpdated(Task task, Guid raisedBy, DateTimeOffset startDate, DateTimeOffset endDate) : base(task,
        raisedBy)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    [JsonProperty] public DateTimeOffset EndDate { get; set; }
    [JsonProperty] public DateTimeOffset StartDate { get; set; }
}