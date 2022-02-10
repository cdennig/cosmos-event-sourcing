using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupGeneralInformationUpdated), 1.0)]
public class GroupGeneralInformationUpdated : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    public GroupGeneralInformationUpdated(Domain.Group group, Guid raisedBy, string name, string description,
        string pictureUri) : base(
        group, raisedBy)
    {
        Name = name;
        Description = description;
        PictureUri = pictureUri;
    }

    private GroupGeneralInformationUpdated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId,
        long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    [JsonProperty] public string Name { get; set; }
    [JsonProperty] public string Description { get; set; }
    [JsonProperty] public string PictureUri { get; set; }
}