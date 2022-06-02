using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupMemberAdded), 1.0)]
public class GroupMemberAdded : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    public GroupMemberAdded(Domain.Group group, Guid raisedBy, Guid memberId) : base(
        group, raisedBy)
    {
        MemberId = memberId;
    }

    private GroupMemberAdded(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid MemberId { get; private set; }
}