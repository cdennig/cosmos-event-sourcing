using ES.Shared.Entity;

namespace Tasks.Domain;

public class TimeLogEntry : AuditableTenantEntity<Guid, Guid, Guid>
{
    private TimeLogEntry(Guid tenantId, Guid id) : base(tenantId, id)
    {
    }

    private TimeLogEntry(Guid tenantId, Guid principalId, Guid id, Task parent, DateOnly day,
        string comment, ulong duration, DateTimeOffset createdAt) : base(tenantId, id)
    {
        Parent = parent;
        Comment = comment;
        Day = day;
        Duration = duration;
        CreatedAt = createdAt;
        CreatedBy = principalId;
    }

    public Task Parent { get; private set; }
    public string Comment { get; private set; }
    public ulong Duration { get; private set; }
    public DateOnly Day { get; private set; }

    public override string ResourceId =>
        (Parent.ProjectId == null || Parent.ProjectId == Guid.Empty)
            ? $"/t/{TenantId}/tasks/{Parent.Id}/timelogs/{Id}"
            : $"/t/{TenantId}/projects/{Parent.ProjectId}/tasks/{Parent.Id}/timelogs/{Id}";

    public static TimeLogEntry Initialize(Guid tenantId, Guid principalId, Guid timeLogId, Task parent,
        DateOnly day,
        string comment, ulong duration, DateTimeOffset createdAt)
    {
        return new TimeLogEntry(tenantId, principalId, timeLogId, parent, day, comment, duration, createdAt);
    }

    public void SetComment(Guid by, string comment)
    {
        Comment = comment;
    }
}