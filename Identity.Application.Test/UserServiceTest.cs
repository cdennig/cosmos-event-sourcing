using Identity.Application.Services;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class UserServiceTest : IClassFixture<UserServiceTestFixture>
{
    private readonly UserServiceTestFixture _fixture;

    public UserServiceTest(UserServiceTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void Test_0001_ReadUser()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.ReadUserFromIdAsync(_fixture.CurrentValidUserId);
        Assert.NotNull(u);
    }

    [Fact]
    public async void Test_0002_IsUserValidAsPrimaryContact_With_Valid_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsPrimaryContact(_fixture.CurrentValidUserId);
        Assert.True(u);
    }

    [Fact]
    public async void Test_0003_IsUserValidAsPrimaryContact_With_Invalid_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsPrimaryContact(_fixture.CurrentInValidUserId);
        Assert.False(u);
    }

    [Fact]
    public async void Test_0004_IsUserValidAsPrimaryContact_With_Deleted_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsPrimaryContact(_fixture.CurrentDeletedUserId);
        Assert.False(u);
    }

    [Fact]
    public async void Test_0005_IsUserValidAsGroupMember_With_Valid_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsGroupMember(_fixture.CurrentValidUserId);
        Assert.True(u);
    }

    [Fact]
    public async void Test_0006_IsUserValidAsGroupMember_With_Invalid_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsGroupMember(_fixture.CurrentInValidUserId);
        Assert.False(u);
    }

    [Fact]
    public async void Test_0007_IsUserValidAsGroupMember_With_Deleted_User()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.IsUserValidAsGroupMember(_fixture.CurrentDeletedUserId);
        Assert.False(u);
    }

    [Fact]
    public async void Test_0008_ReadDeletedUser()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.ReadUserFromIdAsync(_fixture.CurrentDeletedUserId);
        Assert.Null(u);
    }


    [Fact]
    public async void Test_0009_ReadDeletedUser_On_Purpose()
    {
        var usvc = new UserService(_fixture.CurrentRepo);
        var u = await usvc.ReadUserFromIdAsync(_fixture.CurrentDeletedUserId, true);
        Assert.NotNull(u);
        Assert.True(u.Deleted);
    }
}