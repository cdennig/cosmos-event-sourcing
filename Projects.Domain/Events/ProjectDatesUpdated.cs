using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectDatesUpdated : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectDatesUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public ProjectDatesUpdated(Project project, DateTimeOffset startDate, DateTimeOffset endDate) : base(project)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        [JsonProperty] public DateTimeOffset EndDate { get; set; }
        [JsonProperty] public DateTimeOffset StartDate { get; set; }
        
        public override string ResourceId => $"/org/{TenantId}/project/{AggregateId}";
    }
}