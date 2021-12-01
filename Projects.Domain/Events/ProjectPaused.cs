using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectPaused : BaseDomainEvent<Guid, Project, Guid, Guid>
    {
        public ProjectPaused(Project project, Guid raisedBy) : base(project, raisedBy)
        {
            NewStatus = ProjectStatus.Paused;
            OldStatus = project.Status;
        }

        private ProjectPaused(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
    }
}