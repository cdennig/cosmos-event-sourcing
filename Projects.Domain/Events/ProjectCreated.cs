using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectCreated : BaseDomainEvent<Guid, Project, Guid>
    {
        [JsonProperty] public string Title { get; private set; }
        [JsonProperty] public DateTimeOffset StartDate { get; private set; }

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
    }
}