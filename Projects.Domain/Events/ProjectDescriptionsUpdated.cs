using System;
using Newtonsoft.Json;
using ES.Shared.Events;

namespace Projects.Domain.Events
{
    public class ProjectDescriptionsUpdated : BaseDomainEvent<Guid, Project, Guid>
    {
        public ProjectDescriptionsUpdated(Project project, string title, string description) : base(project)
        {
            Title = title;
            Description = description;
        }

        private ProjectDescriptionsUpdated(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        [JsonProperty] public string? Description { get; set; }
        [JsonProperty] public string? Title { get; set; }
    }
}