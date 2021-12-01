using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectUndeleted : BaseDomainEvent<Guid, Project, Guid, Guid>
    {
        private ProjectUndeleted(string aggregateType, Guid tenantId, Guid aggregateId, Guid raisedBy, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
        {
        }

        public ProjectUndeleted(Project project, Guid raisedBy) : base(project, raisedBy)
        {
        }
    }
}