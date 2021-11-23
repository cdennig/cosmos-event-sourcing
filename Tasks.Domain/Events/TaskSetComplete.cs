using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskSetComplete : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskSetComplete(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskSetComplete(Task task) : base(task)
        {
        }
    }
}