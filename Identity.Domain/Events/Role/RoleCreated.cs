using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Role;

[Event(nameof(RoleCreated), 1.0)]
public class RoleCreated : TenantDomainEvent<Guid, Domain.Role, Guid, Guid>
{
    private RoleCreated(string aggregateType, Guid tenantId, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, tenantId, raisedBy, aggregateId, version, timestamp)
    {
    }

    public RoleCreated(Domain.Role role, Guid raisedBy, string name, List<string> actions, List<string> notActions,
        string description, bool isBuiltIn) : base(
        role, raisedBy)
    {
        Name = name;
        Description = description;
        Actions = actions;
        NotActions = notActions;
        IsBuiltIn = isBuiltIn;
    }

    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public string Description { get; private set; }
    [JsonProperty] public bool IsBuiltIn { get; private set; }
    [JsonProperty] public List<string> Actions { get; private set; }
    [JsonProperty] public List<string> NotActions { get; private set; }
}