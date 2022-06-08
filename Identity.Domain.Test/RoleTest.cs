using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Identity.Domain.Test;

[TestCaseOrderer("Identity.Domain.Test.AlphabeticalOrderer", "Identity.Domain.Test")]
public class RoleTest
{
    [Fact]
    public void Test_0001_Role_Create()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newRoleId = Guid.NewGuid();
        var u = Role.Initialize(tenantId, createdBy, newRoleId, "Administrator Role", new List<RoleAction>(),
            new List<RoleAction>(), "", true);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.CreatedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.CreatedBy);
        Assert.Equal("Administrator Role", u.Name);
        Assert.Equal("", u.Description);
        Assert.True(u.IsBuiltIn);
    }

    [Fact]
    public void Test_0002_Role_UpdateGeneralInformation()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newRoleId = Guid.NewGuid();
        var u = Role.Initialize(tenantId, createdBy, newRoleId, "Administrator Role", new List<RoleAction>(),
            new List<RoleAction>(), "", true);
        u.UpdateGeneralInformation(createdBy, "Admins", "Admin Role");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);
        Assert.Equal("Admins", u.Name);
        Assert.Equal("Admin Role", u.Description);
    }

    [Fact]
    public void Test_0003_Role_AssignToGroup()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newRoleId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var u = Role.Initialize(tenantId, createdBy, newRoleId, "Administrator Role", new List<RoleAction>(),
            new List<RoleAction>(), "", true);
        u.AssignRoleToGroup(createdBy, groupId);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);

        Assert.Equal(1, u.RoleAssignments.Count);
        Assert.Equal(groupId, u.RoleAssignments.First().GroupId);
    }

    [Fact]
    public void Test_0004_Role_AssignToGroupAndRemove()
    {
        var createdBy = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var newRoleId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var u = Role.Initialize(tenantId, createdBy, newRoleId, "Administrator Role", new List<RoleAction>(),
            new List<RoleAction>(), "", true);
        u.AssignRoleToGroup(createdBy, groupId);
        u.RemoveRoleFromGroup(createdBy, groupId);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);
        Assert.Equal(0, u.RoleAssignments.Count);
    }
}