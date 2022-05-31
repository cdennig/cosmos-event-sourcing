using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupUndeleted), 1.0)]
public class GroupUndeleted : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    private GroupUndeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public GroupUndeleted(Domain.Group group, Guid raisedBy) : base(
        group, raisedBy)
    {
    }
}