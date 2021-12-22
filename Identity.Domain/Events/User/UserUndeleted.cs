using ES.Shared.Attributes;
using ES.Shared.Events;

namespace Identity.Domain.Events.User;

[Event(nameof(UserUndeleted), 1.0)]
public class UserUndeleted : DomainEvent<Domain.User, Guid, Guid>
{
    private UserUndeleted(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public UserUndeleted(Domain.User user, Guid raisedBy) : base(user, raisedBy)
    {
    }
}