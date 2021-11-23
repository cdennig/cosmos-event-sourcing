using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskDeleted : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskDeleted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskDeleted(Task task) : base(task)
        {
        }
    }
}