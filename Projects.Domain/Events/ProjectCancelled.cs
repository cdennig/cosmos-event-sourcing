using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectCancelled : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectCancelled(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) :
            base(
                aggregateType, tenantId, aggregateId, version, timestamp)
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