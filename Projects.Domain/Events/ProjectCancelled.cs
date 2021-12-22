using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectCancelled), 1.0)]
public class ProjectCancelled : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    private ProjectCancelled(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) :
        base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public ProjectCancelled(Project project, Guid raisedBy) : base(project, raisedBy)
    {
        NewStatus = ProjectStatus.Cancelled;
        OldStatus = project.Status;
    }

    [JsonProperty] public ProjectStatus NewStatus { get; set; }

    [JsonProperty] public ProjectStatus OldStatus { get; set; }
}