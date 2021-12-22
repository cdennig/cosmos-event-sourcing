using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantGeneralInformationUpdated), 1.0)]
public class TenantGeneralInformationUpdated : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantGeneralInformationUpdated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantGeneralInformationUpdated(Domain.Tenant tenant, Guid raisedBy, string name, string description, string pictureUri) : base(tenant, raisedBy)
    {
        Name = name;
        Description = description;
        PictureUri = pictureUri;
        OldName = tenant.Name;
        OldDescription = tenant.Description;
        OldPictureUri = tenant.PictureUri;
    }

    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string PictureUri { get; private set; }
    [JsonProperty] public string OldName { get; private set; }
    [JsonProperty] public string OldDescription { get; private set; }
    [JsonProperty] public string OldPictureUri { get; private set; }
}