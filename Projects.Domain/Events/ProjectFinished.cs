using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectFinished : BaseDomainEvent<Project, Guid>
    {
        public ProjectFinished(Project project, DateTimeOffset actualEndDate) : base(project)
        {
            NewStatus = ProjectStatus.Finished;
            OldStatus = project.Status;
            ActualEndDate = actualEndDate;
        }

        private ProjectFinished(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public ProjectStatus NewStatus { get; set; }
        [JsonProperty] public ProjectStatus OldStatus { get; set; }
        [JsonProperty] public DateTimeOffset ActualEndDate { get; set; }
    }
}