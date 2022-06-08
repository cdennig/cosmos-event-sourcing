using System.Collections.Immutable;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Exceptions;
using Identity.Domain.Events.Role;
using Identity.Domain.Events.Tenant;

namespace Identity.Domain;

public class Role : TenantAggregateRoot<Guid, Role, Guid, Guid>
{
    private Role(Guid tenantId, Guid roleId, IEnumerable<ITenantDomainEvent<Guid, Guid, Guid>> @events) : base(tenantId,
        roleId, @events)
    {
    }

    private Role(Guid tenantId, Guid principalId, Guid roleId,
        string name, List<RoleAction> actions, List<RoleAction> notActions,
        string description = "", bool isBuiltIn = true) : base(tenantId, roleId)
    {
        var uc = new RoleCreated(this, principalId, name, actions, notActions, description, isBuiltIn);
        AddEvent(uc);
    }

    public static Role Initialize(Guid tenantId, Guid principalId, Guid roleId, string name, List<RoleAction> actions,
        List<RoleAction> notActions,
        string description = "", bool isBuiltIn = true)
    {
        return new Role(tenantId, principalId, roleId, name, actions, notActions, description, isBuiltIn);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsBuiltIn { get; private set; }
    public List<RoleAction> Actions { get; private set; }
    public List<RoleAction> NotActions { get; private set; }
    
    private readonly List<RoleAssignment> _roleAssignments = new();
    public IReadOnlyCollection<RoleAssignment> RoleAssignments => _roleAssignments.ToImmutableArray();

    public override string ResourceId => $"/t/{TenantId}/roles/{Id}";

    public void UpdateGeneralInformation(Guid by, string name, string description)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Role readonly");
        var informationUpdated = new RoleGeneralInformationUpdated(this, by, name, description);
        AddEvent(informationUpdated);
    }

    public void AssignRoleToGroup(Guid by, Guid groupId)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Role readonly");
        var roleAssignmentAdded = new RoleAssignmentAdded(this, by,groupId);
        AddEvent(roleAssignmentAdded);
    } 
    
    public void RemoveRoleFromGroup(Guid by, Guid groupId)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Role readonly");
        var roleAssignmentRemoved = new RoleAssignmentRemoved(this, by,groupId);
        AddEvent(roleAssignmentRemoved);
    }
    
    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic)@event);
    }

    private bool IsWritable()
    {
        return true;
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
    
    private void ApplyEvent(RoleGeneralInformationUpdated roleGeneralInformationUpdated)
    {
        Name = roleGeneralInformationUpdated.Name;
        Description = roleGeneralInformationUpdated.Description;
        ModifiedAt = roleGeneralInformationUpdated.Timestamp;
        ModifiedBy = roleGeneralInformationUpdated.RaisedBy;
    }
    
    private void ApplyEvent(RoleAssignmentAdded roleAssignmentAdded)
    {
        var member = new RoleAssignment(this, roleAssignmentAdded.GroupId);
        _roleAssignments.Add(member);
        ModifiedAt = roleAssignmentAdded.Timestamp;
        ModifiedBy = roleAssignmentAdded.RaisedBy;
    }
    
    private void ApplyEvent(RoleAssignmentRemoved roleAssignmentRemoved)
    {
        var assignment = _roleAssignments.Find(m => m.GroupId == roleAssignmentRemoved.GroupId);
        if (assignment != null)
        {
            _roleAssignments.Remove(assignment);
            ModifiedAt = roleAssignmentRemoved.Timestamp;
            ModifiedBy = roleAssignmentRemoved.RaisedBy;
        }
    }
}