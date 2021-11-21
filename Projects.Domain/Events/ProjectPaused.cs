using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectPaused : BaseDomainEvent<Project, Guid>
    {
        public ProjectPaused(Project project) : base(project)
        {
            NewStatus = ProjectStatus.Paused;
            OldStatus = project.Status;
        }

        private ProjectPaused(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
    }
}