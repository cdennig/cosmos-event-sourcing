using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectUndeleted), 1.0)]
public class ProjectUndeleted : BaseDomainEvent<Guid, Project, Guid, Guid>
{
    private ProjectUndeleted(string aggregateType, Guid tenantId, Guid aggregateId, Guid raisedBy, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public ProjectUndeleted(Project project, Guid raisedBy) : base(project, raisedBy)
    {
    }
}