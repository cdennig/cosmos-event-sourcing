using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Role;

[Event(nameof(RoleAssignmentRemoved), 1.0)]
public class RoleAssignmentRemoved : TenantDomainEvent<Guid, Domain.Role, Guid, Guid>
{
    public RoleAssignmentRemoved(Domain.Role group, Guid raisedBy, Guid groupId) : base(
        group, raisedBy)
    {
        GroupId = groupId;
    }

    private RoleAssignmentRemoved(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid GroupId { get; private set; }
}