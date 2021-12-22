using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Identity.Domain.Events.User;

[Event(nameof(UserDeleted), 1.0)]
public class UserDeleted : DomainEvent<Domain.User, Guid, Guid>
{
    private UserDeleted(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserDeleted(Domain.User user, Guid raisedBy) : base(user, raisedBy)
    {
    }
}