using System;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Application.Commands.Role;
using Identity.Domain;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class RoleApplicationTest : IClassFixture<RoleApplicationTestFixture>
{
    private readonly RoleApplicationTestFixture _fixture;

    public RoleApplicationTest(RoleApplicationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateRoleAsync()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateRoleCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Name = "Tenant Admin Role",
            Description = "Tenant Admin Role",
            Actions = new()
            {
                new($"/t", "create"),
                new($"/t/{_fixture.TenantId}", "*"),
                new("/u/*", "read"),
                new($"/u/{_fixture.PrincipalId}", "*"),
                new($"/t/{_fixture.TenantId}/groups", "*"),
                new($"/t/{_fixture.TenantId}/roles", "*"),
                new($"/t/{_fixture.TenantId}/projects", "*"),
                new($"/t/{_fixture.TenantId}/projects/*/tasks", "*")
            },
            NotActions = new(),
            IsBuiltIn = true
        });

        return res.Id;
    }

    private async Task<Role> ReadRoleAsync(Guid tenantId, Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(tenantId, id);
    }

    [Fact]
    async void Test_0001_Create_Role()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateRoleCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Name = "Tenant Admin Role",
            Description = "Tenant Admin Role",
            Actions = new()
            {
                new($"/t", "create"),
                new($"/t/{_fixture.TenantId}", "*"),
                new("/u/*", "read"),
                new($"/u/{_fixture.PrincipalId}", "*"),
                new($"/t/{_fixture.TenantId}/groups", "*"),
                new($"/t/{_fixture.TenantId}/roles", "*"),
                new($"/t/{_fixture.TenantId}/projects", "*"),
                new($"/t/{_fixture.TenantId}/projects/*/tasks", "*")
            },
            NotActions = new(),
            IsBuiltIn = true
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(1, res.Version);
        Assert.NotEmpty(res.ResourceId);
    }

    [Fact]
    async void Test_0002_Create_Role_Validation_Failure()
    {
        var func = async () => await _fixture.CurrentMediator.Send(new CreateRoleCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Name = "Tenant Admin Role",
            Description = "Tenant Admin Role",
            Actions = new(),
            NotActions = new(),
            IsBuiltIn = true
        });

        await Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0003_Assign_Group_To_Role()
    {
        var roleId = await CreateRoleAsync();
        var res = await _fixture.CurrentMediator.Send(new AssignRoleToGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = roleId,
            GroupId = _fixture.CurrentValidGroup
        });
        Assert.NotNull(res);

        var role = await ReadRoleAsync(_fixture.TenantId, res.Id);
        Assert.Equal(1, role.RoleAssignments.Count);
    }

    [Fact]
    async void Test_0004_Remove_Group_From_Role()
    {
        var roleId = await CreateRoleAsync();
        await _fixture.CurrentMediator.Send(new AssignRoleToGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = roleId,
            GroupId = _fixture.CurrentValidGroup
        });

        var role = await ReadRoleAsync(_fixture.TenantId, roleId);
        Assert.Equal(1, role.RoleAssignments.Count);

        await _fixture.CurrentMediator.Send(new RemoveRoleFromGroupCommand()
        {
            TenantId = _fixture.TenantId,
            PrincipalId = _fixture.PrincipalId,
            Id = roleId,
            GroupId = _fixture.CurrentValidGroup
        });

        role = await ReadRoleAsync(_fixture.TenantId, roleId);
        Assert.Equal(0, role.RoleAssignments.Count);
    }
}