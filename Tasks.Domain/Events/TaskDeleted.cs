using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskDeleted : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskDeleted(Task task, Guid raisedBy) : base(task, raisedBy)
        {
        }
    }
}