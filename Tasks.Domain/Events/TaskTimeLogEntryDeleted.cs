using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskTimeLogEntryDeleted : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskTimeLogEntryDeleted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskTimeLogEntryDeleted(Task task, Guid entryId) : base(task)
        {
            TimeLogEntryId = entryId;
        }

        [JsonProperty] public Guid TimeLogEntryId { get; private set; }
    }
}