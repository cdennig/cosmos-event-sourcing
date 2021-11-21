using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectPriorityUpdated : BaseDomainEvent<Project, Guid>
    {
        public ProjectPriorityUpdated(Project project, ProjectPriority priority) : base(project)
        {
            NewPriority = priority;
            OldPriority = project.Priority;
        }

        private ProjectPriorityUpdated(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectPriority NewPriority { get; set; }
        [JsonProperty] public ProjectPriority OldPriority { get; set; }
    }
}