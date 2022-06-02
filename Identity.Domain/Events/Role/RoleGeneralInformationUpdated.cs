using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Role;

[Event(nameof(RoleGeneralInformationUpdated), 1.0)]
public class RoleGeneralInformationUpdated : TenantDomainEvent<Guid, Domain.Role, Guid, Guid>
{
    public RoleGeneralInformationUpdated(Domain.Role role, Guid raisedBy, string name, string description) : base(
        role, raisedBy)
    {
        Name = name;
        Description = description;
    }

    private RoleGeneralInformationUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
}