using System;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Application.Commands.Tenant;
using Identity.Domain;
using Xunit;

namespace Identity.Application.Test;

[TestCaseOrderer("Identity.Application.Test.AlphabeticalOrderer", "Identity.Application.Test")]
public class TenantApplicationTest : IClassFixture<TenantApplicationTestFixture>
{
    private readonly TenantApplicationTestFixture _fixture;

    public TenantApplicationTest(TenantApplicationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateTenantAsync()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Name = "Tenant",
            Description = "Tenant Description",
            Language = "de",
            Location = "EU",
            PictureUri = ""
        });

        return res.Id;
    }

    private async Task<Tenant> ReadTenantAsync(Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(id);
    }

    [Fact]
    async void Test_0001_Create_Tenant()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Name = "Tenant",
            Description = "Tenant Description",
            Language = "de",
            Location = "EU",
            PictureUri = ""
        });

        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(1, res.Version);
        Assert.NotEmpty(res.ResourceId);
    }

    [Fact]
    async void Test_0002_Create_Tenant_Validation_Failure()
    {
        var func = async () => await _fixture.CurrentMediator.Send(new CreateTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Name = "",
            Description = "Tenant Description",
            Language = "de",
            Location = "EU",
            PictureUri = ""
        });

        await Assert.ThrowsAsync<ValidationException>(func);
    }

    [Fact]
    async void Test_0003_Set_Primary_Contact()
    {
        var tenantId = await CreateTenantAsync();
        var res = await _fixture.CurrentMediator.Send(new SetPrimaryContactTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = tenantId,
            ContactId = _fixture.CurrentValidUser
        });
        Assert.NotNull(res);

        var tenant = await ReadTenantAsync(tenantId);
        Assert.Equal(_fixture.CurrentValidUser, tenant.PrimaryContact);
        Assert.Equal(TenantStatus.PrimaryContactAssigned, tenant.Status & TenantStatus.PrimaryContactAssigned);
    }

    [Fact]
    async void Test_0004_Set_Directory_Created()
    {
        var tenantId = await CreateTenantAsync();
        var (adminGroup, usersGroup) = await _fixture.CreateGroupsAsync(tenantId);
        var (adminRole, usersRole) = await _fixture.CreateRolesAsync(tenantId, adminGroup, usersGroup);
        var res = await _fixture.CurrentMediator.Send(new SetDirectoryCreatedTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = tenantId,
            AdminGroupId = adminGroup,
            UsersGroupId = usersGroup,
            AdminRoleId = adminRole,
            UsersRoleId = usersRole
        });
        Assert.NotNull(res);

        var tenant = await ReadTenantAsync(tenantId);
        Assert.Equal(adminGroup, tenant.AdminGroup);
        Assert.Equal(usersGroup, tenant.UsersGroup);
        Assert.Equal(adminRole, tenant.AdminRole);
        Assert.Equal(usersRole, tenant.UsersRole);
        Assert.Equal(TenantStatus.DirectoryCreated, tenant.Status & TenantStatus.DirectoryCreated);
    }

    [Fact]
    async void Test_0005_Full_Setup()
    {
        var tenantId = await CreateTenantAsync();
        var (adminGroup, usersGroup) = await _fixture.CreateGroupsAsync(tenantId);
        var (adminRole, usersRole) = await _fixture.CreateRolesAsync(tenantId, adminGroup, usersGroup);
        var res = await _fixture.CurrentMediator.Send(new SetDirectoryCreatedTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = tenantId,
            AdminGroupId = adminGroup,
            UsersGroupId = usersGroup,
            AdminRoleId = adminRole,
            UsersRoleId = usersRole
        });
        Assert.NotNull(res);

        var tenant = await ReadTenantAsync(tenantId);
        Assert.Equal(adminGroup, tenant.AdminGroup);
        Assert.Equal(usersGroup, tenant.UsersGroup);
        Assert.Equal(adminRole, tenant.AdminRole);
        Assert.Equal(usersRole, tenant.UsersRole);
        Assert.Equal(TenantStatus.DirectoryCreated, tenant.Status & TenantStatus.DirectoryCreated);

        await _fixture.CurrentMediator.Send(new SetPrimaryContactTenantCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            Id = tenantId,
            ContactId = _fixture.CurrentValidUser
        });

        tenant = await ReadTenantAsync(tenantId);
        Assert.Equal(_fixture.CurrentValidUser, tenant.PrimaryContact);
        Assert.Equal(TenantStatus.PrimaryContactAssigned, tenant.Status & TenantStatus.PrimaryContactAssigned);
        Assert.Equal(TenantStatus.DirectoryCreated, tenant.Status & TenantStatus.DirectoryCreated);
    }
}