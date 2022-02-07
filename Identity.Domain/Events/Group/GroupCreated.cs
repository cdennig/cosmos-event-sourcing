using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Group;

[Event(nameof(GroupCreated), 1.0)]
public class GroupCreated : TenantDomainEvent<Guid, Domain.Group, Guid, Guid>
{
    private GroupCreated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public GroupCreated(Domain.Group group, Guid raisedBy, string name, string description, string pictureUri) : base(
        group, raisedBy)
    {
        Name = name;
        Description = description;
        PictureUri = pictureUri;
    }

    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string PictureUri { get; private set; }
}