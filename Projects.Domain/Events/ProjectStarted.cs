using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectStarted : BaseDomainEvent<Guid, Project, Guid>
    {
        public ProjectStarted(Project project, DateTimeOffset actualStartDate) : base(project)
        {
            NewStatus = ProjectStatus.Started;
            OldStatus = project.Status;
            ActualStartDate = actualStartDate;
        }

        private ProjectStarted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
        [JsonProperty] public DateTimeOffset ActualStartDate { get; set; }
    }
}