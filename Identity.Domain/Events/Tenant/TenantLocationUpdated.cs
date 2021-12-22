using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantLocationUpdated), 1.0)]
public class TenantLocationUpdated : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantLocationUpdated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantLocationUpdated(Domain.Tenant tenant, Guid raisedBy, string location) : base(tenant, raisedBy)
    {
        OldLocation = tenant.Location;
        Location = location;
    }

    [JsonProperty] public string Location { get; private set; }
    [JsonProperty] public string OldLocation { get; private set; }
}