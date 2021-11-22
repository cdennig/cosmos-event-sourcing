using System;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Projects.Domain.Events
{
    public class ProjectUndeleted : BaseDomainEvent<Guid, Project, Guid>
    {
        private ProjectUndeleted(string aggregateType, Guid tenantId, Guid aggregateId, long version,
            DateTimeOffset timestamp) : base(
            aggregateType, tenantId, aggregateId, version, timestamp)
        {
        }

        public ProjectUndeleted(Project project) : base(project)
        {
        }
    }
}