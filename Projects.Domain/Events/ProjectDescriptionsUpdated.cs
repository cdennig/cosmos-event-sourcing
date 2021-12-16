using ES.Shared.Attributes;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events;

[Event(nameof(ProjectDescriptionsUpdated), 1.0)]
public class ProjectDescriptionsUpdated : BaseDomainEvent<Guid, Project, Guid, Guid>
{
    public ProjectDescriptionsUpdated(Project project, Guid raisedBy, string title, string description) : base(
        project, raisedBy)
    {
        Title = title;
        Description = description;
    }

    private ProjectDescriptionsUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public string? Description { get; set; }
    [JsonProperty] public string? Title { get; set; }
}