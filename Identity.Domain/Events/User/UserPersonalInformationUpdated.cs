using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.User;

[Event(nameof(UserPersonalInformationUpdated), 1.0)]
public class UserPersonalInformationUpdated : DomainEvent<Domain.User, Guid, Guid>
{
    private UserPersonalInformationUpdated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserPersonalInformationUpdated(Domain.User user, Guid raisedBy, string firstName, string lastName,
        string description, string pictureUri) : base(user, raisedBy)
    {
        FirstName = firstName;
        LastName = lastName;
        Description = description;
        PictureUri = pictureUri;
        
        OldFirstName = user.FirstName;
        OldLastName = user.LastName;
        OldDescription = user.Description;
        OldPictureUri = user.PictureUri;
    }

    [JsonProperty] public string FirstName { get; private set; }
    [JsonProperty] public string LastName { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public string PictureUri { get; private set; }
    [JsonProperty] public string OldFirstName { get; private set; }
    [JsonProperty] public string OldLastName { get; private set; }
    [JsonProperty] public string OldDescription { get; private set; }
    [JsonProperty] public string OldPictureUri { get; private set; }
}