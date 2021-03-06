using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantDirectoryCreated), 1.0)]
public class TenantDirectoryCreated : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantDirectoryCreated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantDirectoryCreated(Domain.Tenant tenant, Guid raisedBy, Guid adminGroupId, Guid usersGroupId,
        Guid adminRoleId, Guid usersRoleId) : base(tenant, raisedBy)
    {
        AdminGroupId = adminGroupId;
        UsersGroupId = usersGroupId;
        AdminRoleId = adminRoleId;
        UsersRoleId = usersRoleId;
    }

    [JsonProperty] public Guid AdminGroupId { get; private set; }
    [JsonProperty] public Guid UsersGroupId { get; private set; }
    [JsonProperty] public Guid AdminRoleId { get; private set; }
    [JsonProperty] public Guid UsersRoleId { get; private set; }
}