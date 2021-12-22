using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.User;

[Event(nameof(UserConfirmed), 1.0)]
public class UserConfirmed : DomainEvent<Domain.User, Guid, Guid>
{
    private UserConfirmed(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserConfirmed(Domain.User user, Guid raisedBy) : base(user, raisedBy)
    {
        OldStatus = user.Status;
    }

    [JsonProperty] public UserStatus OldStatus { get; private set; }
    [JsonProperty] public UserStatus NewStatus { get; private set; }
}