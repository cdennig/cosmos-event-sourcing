using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.User;

[Event(nameof(UserEmailUpdated), 1.0)]
public class UserEmailUpdated : DomainEvent<Domain.User, Guid, Guid>
{
    private UserEmailUpdated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserEmailUpdated(Domain.User user, Guid raisedBy, string email) : base(user, raisedBy)
    {
        OldEmail = user.Email;
        Email = email;
    }

    [JsonProperty] public string OldEmail { get; private set; }
    [JsonProperty] public string Email { get; private set; }
}