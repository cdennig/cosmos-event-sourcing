using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Role;

[Event(nameof(RoleAssignmentAdded), 1.0)]
public class RoleAssignmentAdded : TenantDomainEvent<Guid, Domain.Role, Guid, Guid>
{
    public RoleAssignmentAdded(Domain.Role group, Guid raisedBy, Guid groupId) : base(
        group, raisedBy)
    {
        GroupId = groupId;
    }

    private RoleAssignmentAdded(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public Guid GroupId { get; set; }
}