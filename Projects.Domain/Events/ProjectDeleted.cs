using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectDeleted : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectDeleted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public ProjectDeleted(Project project) : base(project)
        {
        }
    }
}