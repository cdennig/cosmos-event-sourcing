using System;
using Newtonsoft.Json;
using Projects.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectDescriptionsUpdated : BaseDomainEvent<Project, Guid>
    {
        public ProjectDescriptionsUpdated(Project project, string title, string description) : base(project)
        {
            Title = title;
            Description = description;
        }

        private ProjectDescriptionsUpdated(string aggregateType, Guid aggregateId, long version, DateTimeOffset timestamp) : base(
            aggregateType, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public string Description { get; set; }
        [JsonProperty] public string Title { get; set; }
    }
}