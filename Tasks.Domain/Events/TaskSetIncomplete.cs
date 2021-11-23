using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskSetIncomplete : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskSetIncomplete(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskSetIncomplete(Task task) : base(task)
        {
        }
    }
}