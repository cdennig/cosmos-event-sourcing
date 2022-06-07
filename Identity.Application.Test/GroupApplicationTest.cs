using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Commands.Group;
using Identity.Domain;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class GroupApplicationTest : IClassFixture<GroupApplicationTestFixture>
{
    private readonly GroupApplicationTestFixture _fixture;

    public GroupApplicationTest(GroupApplicationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateGroupAsync()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Name = "Test Group",
            Description = "Test Group",
            PictureUri = "https://example.com/picture.jpg"
        });

        return res.Id;
    }

    private async Task<Group> ReadGroupAsync(Guid tenantId, Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(tenantId, id);
    }

    [Fact]
    async void Test_0001_Create_Group()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Name = "Test Group",
            Description = "Test Group",
            PictureUri = "https://example.com/picture.jpg"
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(1, res.Version);
        Assert.NotEmpty(res.ResourceId);
    }

    [Fact]
    async void Test_0002_Create_Group_Validation_Failure()
    {
        Func<Task<CreateGroupCommandResponse>> func = async () =>
        {
            var res = await _fixture.CurrentMediator.Send(new CreateGroupCommand()
            {
                PrincipalId = _fixture.PrincipalId,
                Name = "Test Group",
                Description = "Test Group",
                PictureUri = "https://example.com/picture.jpg"
            });
            return res;
        };

        await Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0003_Add_Member()
    {
        var groupId = await CreateGroupAsync();

        var res = await _fixture.CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = groupId,
            MemberId = _fixture.CurerntValidUser
        });

        var group = await ReadGroupAsync(_fixture.TenantId, groupId);
        Assert.NotNull(group);
        Assert.Equal(1, group.GroupMembers.Count);
        Assert.Equal(_fixture.CurerntValidUser, group.GroupMembers.First().MemberPrincipalId);
    }

    [Fact]
    async void Test_0004_Add_Invalid_Member()
    {
        var groupId = await CreateGroupAsync();

        var func = async () =>
        {
            var res = await _fixture.CurrentMediator.Send(new AddMemberGroupCommand()
            {
                TenantId = _fixture.TenantId,
                PrincipalId = _fixture.PrincipalId,
                Id = groupId,
                MemberId = Guid.NewGuid()
            });
            return res;
        };

        await Assert.ThrowsAsync<Exception>(func);
    }

    [Fact]
    async void Test_0005_Remove_Member()
    {
        var groupId = await CreateGroupAsync();

        await _fixture.CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = groupId,
            MemberId = _fixture.CurerntValidUser
        });

        var group = await ReadGroupAsync(_fixture.TenantId, groupId);
        Assert.NotNull(group);
        Assert.Equal(1, group.GroupMembers.Count);
        Assert.Equal(_fixture.CurerntValidUser, group.GroupMembers.First().MemberPrincipalId);

        await _fixture.CurrentMediator.Send(new RemoveMemberGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = groupId,
            MemberId = _fixture.CurerntValidUser
        });

        group = await ReadGroupAsync(_fixture.TenantId, groupId);
        Assert.NotNull(group);
        Assert.Equal(0, group.GroupMembers.Count);
    }
    
    [Fact]
    async void Test_0006_Remove_Invalid_Member()
    {
        var groupId = await CreateGroupAsync();

        await _fixture.CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = groupId,
            MemberId = _fixture.CurerntValidUser
        });

        var group = await ReadGroupAsync(_fixture.TenantId, groupId);
        Assert.NotNull(group);
        Assert.Equal(1, group.GroupMembers.Count);
        Assert.Equal(_fixture.CurerntValidUser, group.GroupMembers.First().MemberPrincipalId);

        await _fixture.CurrentMediator.Send(new RemoveMemberGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = groupId,
            MemberId = Guid.NewGuid()
        });

        group = await ReadGroupAsync(_fixture.TenantId, groupId);
        Assert.NotNull(group);
        Assert.Equal(1, group.GroupMembers.Count);
    }
}