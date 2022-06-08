using System;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using Identity.Domain;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class UserApplicationTest : IClassFixture<UserApplicationTestFixture>
{
    private readonly UserApplicationTestFixture _fixture;

    public UserApplicationTest(UserApplicationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateUserAsync()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Description = "Test User",
            PictureUri = "https://example.com/picture.jpg"
        });

        return res.Id;
    }

    private async Task<User> ReadUserAsync(Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(id);
    }

    [Fact]
    async void Test_0001_Create_User()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Description = "Test User",
            PictureUri = "https://example.com/picture.jpg"
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(1, res.Version);
        Assert.NotEmpty(res.ResourceId);
    }

    [Fact]
    async void Test_0002_Create_User_Validation_Failure()
    {
        Func<Task<CreateUserCommandResponse>> func = async () =>
        {
            var res = await _fixture.CurrentMediator.Send(new CreateUserCommand()
            {
                PrincipalId = _fixture.PrincipalId,
                FirstName = "",
                LastName = "",
                Email = "jane.doe@example.com",
                Description = "Test User",
                PictureUri = "https://example.com/picture.jpg"
            });
            return res;
        };

        await Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0003_Confirm_User()
    {
        var userId = await CreateUserAsync();

        var res = await _fixture.CurrentMediator.Send(new ConfirmUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId
        });

        var user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.Equal(UserStatus.Confirmed, user.Status);
    }

    [Fact]
    async void Test_0004_Confirm_User_Validation_Failure()
    {
        var userId = await CreateUserAsync();

        Func<Task<ConfirmUserCommandResponse>> func = async () => await _fixture.CurrentMediator.Send(
            new ConfirmUserCommand()
            {
                Id = userId
            });

        Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0006_Update_Email()
    {
        var userId = await CreateUserAsync();

        var res = await _fixture.CurrentMediator.Send(new UpdateEmailUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId,
            Email = "a@b.com"
        });

        var user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.Equal("a@b.com", user.Email);
    }

    [Fact]
    async void Test_0007_Update_Email_Validation_Failure()
    {
        var userId = await CreateUserAsync();

        var func = async () =>
        {
            var res = await _fixture.CurrentMediator.Send(new UpdateEmailUserCommand()
            {
                PrincipalId = _fixture.PrincipalId,
                Id = userId,
                Email = "ab"
            });
            return res;
        };

        await Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0008_Update_PersonalInformation()
    {
        var userId = await CreateUserAsync();

        var res = await _fixture.CurrentMediator.Send(new UpdatePersonalInformationUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId,
            FirstName = "John",
            LastName = "Bar",
            Description = "User Description",
            PictureUri = "https://example.com/pic.jpg"
        });

        var user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Bar", user.LastName);
        Assert.Equal("User Description", user.Description);
        Assert.Equal("https://example.com/pic.jpg", user.PictureUri);
    }

    [Fact]
    async void Test_0009_Update_PersonalInformation_Validation_Failure()
    {
        var userId = await CreateUserAsync();

        var func = async () => await _fixture.CurrentMediator.Send(new UpdatePersonalInformationUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId,
            FirstName =
                "0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345",
            LastName = "Bar",
            Description = "User Description",
            PictureUri = "https://example.com/pic.jpg"
        });

        await Assert.ThrowsAsync<ValidationException>(func);
    }


    [Fact]
    async void Test_0010_Delete_User()
    {
        var userId = await CreateUserAsync();

        var res = await _fixture.CurrentMediator.Send(new DeleteUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId
        });

        var user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.True(user.Deleted);
    }

    [Fact]
    async void Test_0010_Undelete_User()
    {
        var userId = await CreateUserAsync();

        await _fixture.CurrentMediator.Send(new DeleteUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId
        });

        var user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.True(user.Deleted);

        await _fixture.CurrentMediator.Send(new UndeleteUserCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = userId
        });

        user = await ReadUserAsync(userId);
        Assert.NotNull(user);
        Assert.False(user.Deleted);
    }
}