using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantCreated), 1.0)]
public class TenantCreated : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantCreated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantCreated(Domain.Tenant tenant, Guid raisedBy, string name, string description, string language,
        string location, string pictureUri) : base(tenant, raisedBy)
    {
        Name = name;
        Description = description;
        Language = language;
        Location = location;
        PictureUri = pictureUri;
    }

    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string Location { get; private set; }
    [JsonProperty] public string Language { get; private set; }
    [JsonProperty] public string PictureUri { get; private set; }
}