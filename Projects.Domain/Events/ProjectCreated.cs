using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectCreated : BaseDomainEvent<Guid, Project, Guid, Guid>
    {
        private ProjectCreated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public ProjectCreated(Project project, Guid raisedBy, string title, string description,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate, ProjectPriority priority = ProjectPriority.Medium) : base(project, raisedBy)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Priority = priority;
        }

        [JsonProperty] public string? Title { get; private set; }
        [JsonProperty] public string? Description { get; private set; }
        [JsonProperty] public DateTimeOffset? StartDate { get; private set; }
        [JsonProperty] public DateTimeOffset? EndDate { get; private set; }
        [JsonProperty] public ProjectPriority Priority { get; private set; }
    }
}