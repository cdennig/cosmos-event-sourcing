using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectCreated : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectCreated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public ProjectCreated(Project project, string title, string description, DateTimeOffset? startDate,
            DateTimeOffset? endDate, ProjectPriority priority = ProjectPriority.Medium) : base(project)
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