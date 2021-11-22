using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectPriorityUpdated : BaseDomainEvent<Guid, Project, Guid>
    {
        public ProjectPriorityUpdated(Project project, ProjectPriority priority) : base(project)
        {
            NewPriority = priority;
            OldPriority = project.Priority;
        }

        private ProjectPriorityUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectPriority NewPriority { get; set; }
        [JsonProperty] public ProjectPriority OldPriority { get; set; }
    }
}