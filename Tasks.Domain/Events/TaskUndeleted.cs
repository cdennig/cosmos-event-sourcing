using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskUndeleted : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskUndeleted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskUndeleted(Task task) : base(task)
        {
        }
    }
}