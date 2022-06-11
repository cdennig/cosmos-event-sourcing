using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupDeleted), 1.0)]
public class GroupDeleted : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    private GroupDeleted(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public GroupDeleted(Domain.Group group, Guid raisedBy) : base(
        group, raisedBy)
    {
    }
}