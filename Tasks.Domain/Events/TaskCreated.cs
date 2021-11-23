using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskCreated : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskCreated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskCreated(Task task, string title, string description, Guid? projectId, DateTimeOffset? startDate,
            DateTimeOffset? endDate, TaskPriority priority = TaskPriority.Medium) : base(task)
        {
            Title = title;
            Description = description;
            ProjectId = projectId;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
        }

        [JsonProperty] public string Title { get; private set; }
        [JsonProperty] public string Description { get; private set; }
        [JsonProperty] public Guid? ProjectId { get; private set; }
        [JsonProperty] public DateTimeOffset? StartDate { get; private set; }
        [JsonProperty] public DateTimeOffset? EndDate { get; private set; }
        [JsonProperty] public TaskPriority Priority { get; private set; }
    }
}