using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events;

public class TenantUserCreated : BaseDomainEvent<Guid, TenantUser, Guid, Guid>
{
    private TenantUserCreated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantUserCreated(TenantUser user, Guid raisedBy, string firstName, string lastName, string email,
        string description, string pictureUri) : base(user, raisedBy)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Description = description;
        PictureUri = pictureUri;
    }

    [JsonProperty] public string? FirstName { get; private set; }
    [JsonProperty] public string? LastName { get; private set; }
    [JsonProperty] public string? Description { get; private set; }
    [JsonProperty] public string? Email { get; private set; }
    [JsonProperty] public string? PictureUri { get; private set; }
}