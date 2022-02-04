using ES.Shared.Aggregate;
using ES.Shared.Events;

namespace Identity.Domain;

public class Group : TenantAggregateRoot<Guid, Group, Guid, Guid>
{
    private Group(Guid tenantId, Guid groupId) : base(tenantId, groupId)
    {
    }

    private Group(Guid tenantId, Guid principalId, Guid groupId,
        string name,
        string description = "",
        string pictureUri = "") : base(tenantId, groupId)
    {
        // var uc = new GroupCreated(this, principalId, firstName,
        //     lastName, email, description, pictureUri);
        // AddEvent(uc);
    }

    public override string ResourceId => $"/t/{TenantId}/groups/{Id}";

    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        throw new NotImplementedException();
    }
}