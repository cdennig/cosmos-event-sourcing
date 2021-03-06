using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectResumed), 1.0)]
public class ProjectResumed : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    public ProjectResumed(Project project, Guid raisedBy) : base(project, raisedBy)
    {
        NewStatus = ProjectStatus.Resumed;
        OldStatus = project.Status;
    }

    private ProjectResumed(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public ProjectStatus NewStatus { get; set; }

    [JsonProperty] public ProjectStatus OldStatus { get; set; }
}