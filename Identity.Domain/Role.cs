using ES.Shared.Aggregate;
using ES.Shared.Events;
using Identity.Domain.Events.Role;

namespace Identity.Domain;

public class Role : TenantAggregateRoot<Guid, Role, Guid, Guid>
{
    private Role(Guid tenantId, Guid roleId, IEnumerable<ITenantDomainEvent<Guid, Guid, Guid>> @events) : base(tenantId,
        roleId, @events)
    {
    }

    private Role(Guid tenantId, Guid principalId, Guid roleId,
        string name, List<string> actions, List<string> notActions,
        string description = "", bool isBuiltIn = true) : base(tenantId, roleId)
    {
        var uc = new RoleCreated(this, principalId, name, actions, notActions, description, isBuiltIn);
        AddEvent(uc);
    }

    public static Role Initialize(Guid tenantId, Guid principalId, Guid roleId, string name, List<string> actions,
        List<string> notActions,
        string description = "", bool isBuiltIn = true)
    {
        return new Role(tenantId, principalId, roleId, name, actions, notActions, description, isBuiltIn);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsBuiltIn { get; private set; }
    public List<string> Actions { get; private set; }
    public List<string> NotActions { get; private set; }


    public override string ResourceId => $"/t/{TenantId}/roles/{Id}";

    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic)@event);
    }

    private bool IsWritable()
    {
        return false;
    }

    private void ApplyEvent(RoleCreated roleCreated)
    {
        Name = roleCreated.Name;
        Description = roleCreated.Description;
        IsBuiltIn = roleCreated.IsBuiltIn;
        Actions = roleCreated.Actions;
        NotActions = roleCreated.NotActions;
        CreatedAt = roleCreated.Timestamp;
        CreatedBy = roleCreated.RaisedBy;
    }
}