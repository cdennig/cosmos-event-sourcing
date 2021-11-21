using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectStarted : BaseDomainEvent<Project, Guid>
    {
        public ProjectStarted(Project project, DateTimeOffset actualStartDate) : base(project)
        {
            NewStatus = ProjectStatus.Started;
            OldStatus = project.Status;
            ActualStartDate = actualStartDate;
        }

        private ProjectStarted(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
        [JsonProperty] public DateTimeOffset ActualStartDate { get; set; }
    }
}