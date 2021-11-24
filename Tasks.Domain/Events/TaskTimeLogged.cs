using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskTimeLogged : BaseDomainEvent<Guid, Task, Guid>
    {
        private TaskTimeLogged(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public TaskTimeLogged(Task task, DateOnly day,
            string comment, ulong duration) : base(task)
        {
            TimeLogEntryId = Guid.NewGuid();
            Comment = comment;
            Day = day;
            Duration = duration;
        }

        [JsonProperty] public Guid TimeLogEntryId { get; private set; }
        [JsonProperty] public string Comment { get; private set; }
        [JsonProperty] public ulong Duration { get; private set; }
        [JsonProperty] public DateOnly Day { get; private set; }
    }
}