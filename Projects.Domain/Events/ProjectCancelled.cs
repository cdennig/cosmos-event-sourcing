using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectCancelled : BaseDomainEvent<Project, Guid>
    {
        private ProjectCancelled(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) :
            base(
                aggregateType, aggregateId, version, timestamp)
        {
        }

        public ProjectCancelled(Project project) : base(project)
        {
            NewStatus = ProjectStatus.Cancelled;
            OldStatus = project.Status;
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }

        [JsonProperty] public ProjectStatus OldStatus { get; set; }
    }
}