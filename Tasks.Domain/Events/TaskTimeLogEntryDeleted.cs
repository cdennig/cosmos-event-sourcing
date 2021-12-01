using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskTimeLogEntryDeleted : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskTimeLogEntryDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
            long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskTimeLogEntryDeleted(Task task, Guid entryId, Guid raisedBy) : base(task, raisedBy)
        {
            TimeLogEntryId = entryId;
        }

        [JsonProperty] public Guid TimeLogEntryId { get; private set; }
    }
}