using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events;

[Event(nameof(TaskEstimated), 1.0)]
public class TaskEstimated : BaseDomainEvent<Guid, Task, Guid, Guid>
{
    public TaskEstimated(Task task, Guid raisedBy, ulong estimation) : base(task, raisedBy)
    {
        NewTimeEstimation = estimation;
        OldTimeEstimation = task.TimeEstimation;
    }

    private TaskEstimated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public ulong NewTimeEstimation { get; set; }
    [JsonProperty] public ulong OldTimeEstimation { get; set; }
}