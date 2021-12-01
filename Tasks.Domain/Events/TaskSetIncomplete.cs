using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskSetIncomplete : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskSetIncomplete(string aggregateType, Guid tenantId, Guid aggregateId, Guid raisedBy, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskSetIncomplete(Task task, Guid raisedBy) : base(task, raisedBy)
        {
        }
    }
}