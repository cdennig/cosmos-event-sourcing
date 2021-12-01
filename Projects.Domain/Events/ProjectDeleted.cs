using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectDeleted : BaseDomainEvent<Guid, Project, Guid, Guid>
    {
        private ProjectDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public ProjectDeleted(Project project, Guid raisedBy) : base(project, raisedBy)
        {
        }
    }
}