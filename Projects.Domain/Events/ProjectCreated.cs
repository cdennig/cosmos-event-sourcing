using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectCreated : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectCreated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public ProjectCreated(Project project, string title, DateTimeOffset startDate) : base(project)
        {
            Title = title;
            StartDate = startDate;
        }

        [JsonProperty] public string Title { get; private set; }
        [JsonProperty] public DateTimeOffset StartDate { get; private set; }
        public override string ResourceId => $"/org/{TenantId}/project/{AggregateId}";
    }
}