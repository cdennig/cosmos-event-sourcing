using System.Collections.Immutable;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Exceptions;
using Identity.Domain.Events.Group;

namespace Identity.Domain;

public class Group : TenantAggregateRoot<Guid, Group, Guid, Guid>
{
    private Group(Guid tenantId, Guid groupId, IEnumerable<ITenantDomainEvent<Guid, Guid, Guid>> @events) : base(
        tenantId, groupId, @events)
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

    private readonly List<GroupMember> _groupMembers = new();
    public IReadOnlyCollection<GroupMember> GroupMembers => _groupMembers.ToImmutableArray();

    public override string ResourceId => $"/t/{TenantId}/groups/{Id}";

    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic)@event);
    }

    private bool IsWritable()
    {
        return !Deleted;
    }

    public void UpdateGeneralInformation(Guid by, string name, string description, string pictureUri)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Group is read-only.");

        var giu = new GroupGeneralInformationUpdated(this, by, name, description, pictureUri);
        AddEvent(giu);
    }

    public void AddGroupMember(Guid by, Guid memberId)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Group is read-only.");
        var gma = new GroupMemberAdded(this, by, memberId);
        AddEvent(gma);
    }

    public void RemoveGroupMember(Guid by, Guid memberId)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Group is read-only.");
        var gmr = new GroupMemberRemoved(this, by, memberId);
        AddEvent(gmr);
    }

    private void ApplyEvent(GroupCreated groupCreated)
    {
        Name = groupCreated.Name;
        Description = groupCreated.Description;
        PictureUri = groupCreated.PictureUri;
        CreatedAt = groupCreated.Timestamp;
        CreatedBy = groupCreated.RaisedBy;
    }

    private void ApplyEvent(GroupGeneralInformationUpdated groupGeneralInformationUpdated)
    {
        Name = groupGeneralInformationUpdated.Name;
        Description = groupGeneralInformationUpdated.Description;
        ModifiedAt = groupGeneralInformationUpdated.Timestamp;
        ModifiedBy = groupGeneralInformationUpdated.RaisedBy;
    }

    private void ApplyEvent(GroupMemberAdded groupMemberAdded)
    {
        var member = new GroupMember(this, groupMemberAdded.MemberId);
        _groupMembers.Add(member);
        ModifiedAt = groupMemberAdded.Timestamp;
        ModifiedBy = groupMemberAdded.RaisedBy;
    }

    private void ApplyEvent(GroupMemberRemoved groupMemberRemoved)
    {
        var member = _groupMembers.Find(m => m.memberId == groupMemberRemoved.MemberId);
        if (member != null)
        {
            _groupMembers.Remove(member);
            ModifiedAt = groupMemberRemoved.Timestamp;
            ModifiedBy = groupMemberRemoved.RaisedBy;
        }
    }
}