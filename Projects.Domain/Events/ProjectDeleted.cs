using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectDeleted), 1.0)]
public class ProjectDeleted : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    private ProjectDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public ProjectDeleted(Project project, Guid raisedBy) : base(project, raisedBy)
    {
    }
}