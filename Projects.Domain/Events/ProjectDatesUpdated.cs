using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectDatesUpdated), 1.0)]
public class ProjectDatesUpdated : TenantDomainEvent<Guid, Project, Guid, Guid>
{
    private ProjectDatesUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public ProjectDatesUpdated(Project project, Guid raisedBy, DateTimeOffset startDate, DateTimeOffset endDate) :
        base(project, raisedBy)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    [JsonProperty] public DateTimeOffset EndDate { get; set; }
    [JsonProperty] public DateTimeOffset StartDate { get; set; }
}