using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectResumed : BaseDomainEvent<Project, Guid>
    {
        public ProjectResumed(Project project) : base(project)
        {
            NewStatus = ProjectStatus.Resumed;
            OldStatus = project.Status;
        }

        private ProjectResumed(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }

        [JsonProperty] public ProjectStatus OldStatus { get; set; }
    }
}