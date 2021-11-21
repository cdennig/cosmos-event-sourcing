using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectDatesUpdated : BaseDomainEvent<Project, Guid>
    {
        private ProjectDatesUpdated(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        public ProjectDatesUpdated(Project project, DateTimeOffset startDate, DateTimeOffset endDate) : base(project)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        [JsonProperty] public DateTimeOffset EndDate { get; set; }
        [JsonProperty] public DateTimeOffset StartDate { get; set; }
    }
}