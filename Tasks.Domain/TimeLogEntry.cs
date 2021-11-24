using System;
using ES.Shared.Entity;

namespace Tasks.Domain
{
    public class TimeLogEntry : DomainEntity<Guid, Guid>
    {
        private TimeLogEntry(Guid tenantId, Guid id) : base(tenantId, id)
        {
        }

        private TimeLogEntry(Guid tenantId, Guid id, Guid parentId, DateOnly day,
            string comment, ulong duration, DateTimeOffset createdAt) : base(tenantId, id)
        {
            Parent = parentId;
            Comment = comment;
            Day = day;
            Duration = duration;
            CreatedAt = createdAt;
        }

        public Guid Parent { get; private set; }
        public string Comment { get; private set; }
        public ulong Duration { get; private set; }
        public DateOnly Day { get; private set; }

        public override string ResourceId { get; }

        public static TimeLogEntry Initialize(Guid tenantId, Guid timeLogId, Guid parentId, DateOnly day,
            string comment, ulong duration, DateTimeOffset createdAt)
        {
            return new TimeLogEntry(tenantId, timeLogId, parentId, day, comment, duration, createdAt);
        }
    }
}