using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectFinished), 1.0)]
public class ProjectFinished : BaseDomainEvent<Guid, Project, Guid, Guid>
{
    public ProjectFinished(Project project, Guid raisedBy, DateTimeOffset actualEndDate) : base(project, raisedBy)
    {
        NewStatus = ProjectStatus.Finished;
        OldStatus = project.Status;
        ActualEndDate = actualEndDate;
    }

    private ProjectFinished(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public ProjectStatus NewStatus { get; set; }
    [JsonProperty] public ProjectStatus OldStatus { get; set; }
    [JsonProperty] public DateTimeOffset ActualEndDate { get; set; }
}