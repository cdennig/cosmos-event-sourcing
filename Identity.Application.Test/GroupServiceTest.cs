using Identity.Application.Services;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class GroupServiceTest : IClassFixture<GroupServiceTestFixture>
{
    private readonly GroupServiceTestFixture _fixture;

    public GroupServiceTest(GroupServiceTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void Test_0001_ReadGroup()
    {
        var usvc = new GroupService(_fixture.CurrentRepo);
        var u = await usvc.ReadGroupFromIdAsync(_fixture.TenantId, _fixture.CurrentValidGroupId);
        Assert.NotNull(u);
    }

    [Fact]
    public async void Test_0002_IsGroupValidForRoleAssignment_With_Valid_Group()
    {
        var usvc = new GroupService(_fixture.CurrentRepo);
        var u = await usvc.IsGroupValidForRoleAssignment(_fixture.TenantId, _fixture.CurrentValidGroupId);
        Assert.True(u);
    }


    [Fact]
    public async void Test_0003_IsGroupValidForRoleAssignment_With_Deleted_Group()
    {
        var usvc = new GroupService(_fixture.CurrentRepo);
        var u = await usvc.IsGroupValidForRoleAssignment(_fixture.TenantId, _fixture.CurrentDeletedGroupId);
        Assert.False(u);
    }
    
    [Fact]
    public async void Test_0004_ReadDeletedGroup()
    {
        var usvc = new GroupService(_fixture.CurrentRepo);
        var u = await usvc.ReadGroupFromIdAsync(_fixture.TenantId, _fixture.CurrentDeletedGroupId);
        Assert.Null(u);
    }
    
    [Fact]
    public async void Test_0005_ReadDeletedGroup_On_Purpose()
    {
        var usvc = new GroupService(_fixture.CurrentRepo);
        var u = await usvc.ReadGroupFromIdAsync(_fixture.TenantId, _fixture.CurrentDeletedGroupId, true);
        Assert.NotNull(u);
        Assert.True(u.Deleted);
    }
}