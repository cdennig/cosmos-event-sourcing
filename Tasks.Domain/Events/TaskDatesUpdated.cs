using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskDatesUpdated : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskDatesUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskDatesUpdated(Task task, DateTimeOffset startDate, DateTimeOffset endDate) : base(task)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        [JsonProperty] public DateTimeOffset EndDate { get; set; }
        [JsonProperty] public DateTimeOffset StartDate { get; set; }
    }
}