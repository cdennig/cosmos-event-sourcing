using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Tasks.Domain.Events
{
    public class TaskEstimated : BaseDomainEvent<Guid, Task, Guid>
    {
        public TaskEstimated(Task task, ulong estimation) : base(task)
        {
            NewTimeEstimation = estimation;
            OldTimeEstimation = task.TimeEstimation;
        }

        private TaskEstimated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ulong NewTimeEstimation { get; set; }
        [JsonProperty] public ulong OldTimeEstimation { get; set; }
    }
}