using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectFinished : BaseDomainEvent<Guid, Project, Guid>
    {
        public ProjectFinished(Project project, DateTimeOffset actualEndDate) : base(project)
        {
            NewStatus = ProjectStatus.Finished;
            OldStatus = project.Status;
            ActualEndDate = actualEndDate;
        }

        private ProjectFinished(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
        [JsonProperty] public DateTimeOffset ActualEndDate { get; set; }
        public override string ResourceId => $"/org/{TenantId}/project/{AggregateId}";
    }
}