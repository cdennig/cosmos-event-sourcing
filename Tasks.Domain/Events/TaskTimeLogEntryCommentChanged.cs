using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Tasks.Domain.Events
{
    public class TaskTimeLogEntryCommentChanged : BaseDomainEvent<Guid, Task, Guid, Guid>
    {
        private TaskTimeLogEntryCommentChanged(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
            long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public TaskTimeLogEntryCommentChanged(Task task, TimeLogEntry entry, Guid raisedBy, string comment) : base(task,
            raisedBy)
        {
            TimeLogEntryId = entry.Id;
            NewComment = comment;
            OldComment = entry.Comment;
        }

        [JsonProperty] public Guid TimeLogEntryId { get; private set; }
        [JsonProperty] public string NewComment { get; }
        [JsonProperty] public string OldComment { get; }
    }
}