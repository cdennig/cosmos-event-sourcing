using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskUndeleted : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskUndeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskUndeleted(Task task, Guid raisedBy) : base(task, raisedBy)
        {
        }
    }
}