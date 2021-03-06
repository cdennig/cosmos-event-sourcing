using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events;

[Event(nameof(TaskCreated), 1.0)]
public class TaskCreated : TenantDomainEvent<Guid, Task, Guid, Guid>
{
    private TaskCreated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TaskCreated(Task task, Guid raisedBy, string title, string description, Guid? projectId,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate, TaskPriority priority = TaskPriority.Medium, ulong timeEstimation = 0) : base(task,
        raisedBy)
    {
        Title = title;
        Description = description;
        ProjectId = projectId;
        StartDate = startDate;
        EndDate = endDate;
        Priority = priority;
        TimeEstimation = timeEstimation;
    }

    [JsonProperty] public string? Title { get; private set; }
    [JsonProperty] public string? Description { get; private set; }
    [JsonProperty] public Guid? ProjectId { get; private set; }
    [JsonProperty] public DateTimeOffset? StartDate { get; private set; }
    [JsonProperty] public DateTimeOffset? EndDate { get; private set; }
    [JsonProperty] public TaskPriority? Priority { get; private set; }
    [JsonProperty] public ulong TimeEstimation { get; private set; }
}