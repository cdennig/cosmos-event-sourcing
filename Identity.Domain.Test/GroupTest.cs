using System;
using System.Linq;
using Xunit;

namespace Identity.Domain.Test;

[TestCaseOrderer("Identity.Domain.Test.AlphabeticalOrderer", "Identity.Domain.Test")]
public class GroupTest
{
    [Fact]
    public void Test_0001_Group_Create()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.CreatedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.CreatedBy);
        Assert.Equal("Administrators", u.Name);
        Assert.Equal("Member of this group are org admins.", u.Description);
        Assert.Empty(u.PictureUri);
        Assert.Equal(0, u.GroupMembers.Count);
    }

    [Fact]
    public void Test_0002_Group_UpdateGeneralInformation()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.UpdateGeneralInformation(createdBy, "Admins", "Admin Group", "");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);
        Assert.Equal("Admins", u.Name);
        Assert.Equal("Admin Group", u.Description);
        Assert.Empty(u.PictureUri);
        Assert.Equal(0, u.GroupMembers.Count);
    }

    [Fact]
    public void Test_0003_Group_AddMember()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.AddGroupMember(createdBy, createdBy);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);

        Assert.Equal(1, u.GroupMembers.Count);
        Assert.Equal(createdBy, u.GroupMembers.First().memberPrincipalId);
    }

    [Fact]
    public void Test_0004_Group_AddAndRemoveMember()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.AddGroupMember(createdBy, createdBy);
        u.RemoveGroupMember(createdBy, createdBy);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);

        Assert.Equal(0, u.GroupMembers.Count);
    }

    [Fact]
    public void Test_0005_Group_RemoveMemberNotInList()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.RemoveGroupMember(createdBy, createdBy);
        Assert.NotEqual(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.NotEqual(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);

        Assert.Equal(0, u.GroupMembers.Count);
    }

    [Fact]
    public void Test_0006_Group_Delete()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.DeleteGroup(createdBy);
        Assert.True(u.Deleted);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.DeletedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.DeletedBy);
    }

    [Fact]
    public void Test_0007_Group_Undelete()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newGroupId = Guid.NewGuid();
        var u = Group.Initialize(tenantId, createdBy, newGroupId, "Administrators",
            "Member of this group are org admins.",
            "");
        u.DeleteGroup(createdBy);
        u.UndeleteGroup(createdBy);
        Assert.False(u.Deleted);
    }
}