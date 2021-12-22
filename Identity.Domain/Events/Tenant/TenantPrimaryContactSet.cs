using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantPrimaryContactSet), 1.0)]
public class TenantPrimaryContactSet : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantPrimaryContactSet(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantPrimaryContactSet(Domain.Tenant tenant, Guid raisedBy, Guid contactId) : base(tenant, raisedBy)
    {
        OldPrimaryContact = tenant.PrimaryContact;
        PrimaryContact = contactId;
    }

    [JsonProperty] public Guid PrimaryContact { get; private set; }
    [JsonProperty] public Guid? OldPrimaryContact { get; private set; }
}