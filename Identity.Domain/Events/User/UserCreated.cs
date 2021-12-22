using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.User;

[Event(nameof(UserCreated), 1.0)]
public class UserCreated : DomainEvent<Domain.User, Guid, Guid>
{
    private UserCreated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserCreated(Domain.User user, Guid raisedBy, string firstName, string lastName, string email,
        string description, string pictureUri) : base(user, raisedBy)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Description = description;
        PictureUri = pictureUri;
    }

    [JsonProperty] public string FirstName { get; private set; }
    [JsonProperty] public string LastName { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string Email { get; private set; }
    [JsonProperty] public string PictureUri { get; private set; }
}