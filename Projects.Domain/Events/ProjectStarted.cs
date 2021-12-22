using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectStarted), 1.0)]
public class ProjectStarted : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    public ProjectStarted(Project project, Guid raisedBy, DateTimeOffset actualStartDate) : base(project, raisedBy)
    {
        NewStatus = ProjectStatus.Started;
        OldStatus = project.Status;
        ActualStartDate = actualStartDate;
    }

    private ProjectStarted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public ProjectStatus NewStatus { get; set; }
    [JsonProperty] public ProjectStatus OldStatus { get; set; }
    [JsonProperty] public DateTimeOffset ActualStartDate { get; set; }
}