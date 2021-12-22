using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectPriorityUpdated), 1.0)]
public class ProjectPriorityUpdated : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    public ProjectPriorityUpdated(Project project, Guid raisedBy, ProjectPriority priority) : base(project,
        raisedBy)
    {
        NewPriority = priority;
        OldPriority = project.Priority;
    }

    private ProjectPriorityUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public ProjectPriority NewPriority { get; set; }
    [JsonProperty] public ProjectPriority OldPriority { get; set; }
}