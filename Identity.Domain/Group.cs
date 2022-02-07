using ES.Shared.Aggregate;
using ES.Shared.Events;
using Identity.Domain.Events.Group;

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
        var uc = new GroupCreated(this, principalId, name, description, pictureUri);
        AddEvent(uc);
    }

    public static Group Initialize(Guid tenantId, Guid principalId, Guid groupId, string name,
        string description = "", string pictureUri = "")
    {
        return new Group(tenantId, principalId, groupId, name, description, pictureUri);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PictureUri { get; private set; }

    public override string ResourceId => $"/t/{TenantId}/groups/{Id}";

    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic) @event);
    }

    private bool IsWritable()
    {
        return !Deleted;
    }

    private void ApplyEvent(GroupCreated groupCreated)
    {
        Name = groupCreated.Name;
        Description = groupCreated.Description;
        PictureUri = groupCreated.PictureUri;
        CreatedAt = groupCreated.Timestamp;
        CreatedBy = groupCreated.RaisedBy;
    }
}