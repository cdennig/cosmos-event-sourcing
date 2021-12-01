using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskSetComplete : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskSetComplete(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskSetComplete(Task task, Guid raisedBy) : base(task, raisedBy)
        {
        }
    }
}