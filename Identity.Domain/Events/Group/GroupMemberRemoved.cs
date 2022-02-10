using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupMemberRemoved), 1.0)]
public class GroupMemberRemoved : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    public GroupMemberRemoved(Domain.Group group, Guid raisedBy, Guid memberId) : base(
        group, raisedBy)
    {
        MemberId = memberId;
    }

    private GroupMemberRemoved(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid MemberId { get; set; }
}