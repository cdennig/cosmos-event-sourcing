using System;
using System.Linq;
using Xunit;

namespace Identity.Domain.Test;

[TestCaseOrderer("Identity.Domain.Test.AlphabeticalOrderer", "Identity.Domain.Test")]
public class TenantTest
{
    [Fact]
    public void Test_0001_Tenant_Create()
    {
        var createdBy = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenant = Tenant.Initialize(createdBy, newTenantId, "My New Tenant", "A new tenant/organization.",
            "de-DE", "EU", "");
        Assert.Equal(tenant.DomainEvents.Last().Timestamp, tenant.CreatedAt);
        Assert.Equal(tenant.DomainEvents.Last().RaisedBy, tenant.CreatedBy);
        Assert.Equal(TenantStatus.Requested, tenant.Status);
        Assert.Equal("My New Tenant", tenant.Name);
        Assert.Equal("A new tenant/organization.", tenant.Description);
        Assert.Equal("EU", tenant.Location);
        Assert.Equal("de-DE", tenant.Language);
        Assert.Empty(tenant.PictureUri);
    }

    [Fact]
    public void Test_0002_Tenant_Update_GeneralInformation()
    {
        var createdBy = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenant = Tenant.Initialize(createdBy, newTenantId, "My New Tenant", "A new tenant/organization.",
            "de-DE", "EU", "");
        tenant.UpdateGeneralInformation(createdBy, "CompTenant", "Comp Description", "/uri/123.png");
        Assert.Equal(tenant.DomainEvents.Last().Timestamp, tenant.ModifiedAt);
        Assert.Equal(createdBy, tenant.ModifiedBy);
        Assert.Equal("CompTenant", tenant.Name);
        Assert.Equal("Comp Description", tenant.Description);
        Assert.Equal("/uri/123.png", tenant.PictureUri);
    }

    [Fact]
    public void Test_0003_Tenant_Update_Location()
    {
        var createdBy = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenant = Tenant.Initialize(createdBy, newTenantId, "My New Tenant", "A new tenant/organization.",
            "de-DE", "EU", "");
        tenant.UpdateLocation(createdBy, "US");
        Assert.Equal(tenant.DomainEvents.Last().Timestamp, tenant.ModifiedAt);
        Assert.Equal(createdBy, tenant.ModifiedBy);
        Assert.Equal("US", tenant.Location);
    }

    [Fact]
    public void Test_0004_Tenant_Update_Language()
    {
        var createdBy = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenant = Tenant.Initialize(createdBy, newTenantId, "My New Tenant", "A new tenant/organization.",
            "de-DE", "EU", "");
        tenant.UpdateLanguage(createdBy, "en-US");
        Assert.Equal(tenant.DomainEvents.Last().Timestamp, tenant.ModifiedAt);
        Assert.Equal(createdBy, tenant.ModifiedBy);
        Assert.Equal("en-US", tenant.Language);
    }
    
    [Fact]
    public void Test_0005_Tenant_Set_PrimaryContact()
    {
        var createdBy = Guid.NewGuid();
        var newTenantId = Guid.NewGuid();
        var tenant = Tenant.Initialize(createdBy, newTenantId, "My New Tenant", "A new tenant/organization.",
            "de-DE", "EU", "");
        tenant.UpdateLanguage(createdBy, "en-US");
        Assert.Equal(tenant.DomainEvents.Last().Timestamp, tenant.ModifiedAt);
        Assert.Equal(createdBy, tenant.ModifiedBy);
        Assert.Equal("en-US", tenant.Language);
    }

    // [Fact]
    // public void Test_0005_User_Delete()
    // {
    //     var createdBy = Guid.NewGuid();
    //     var newUserId = Guid.NewGuid();
    //     var u = User.Initialize(createdBy, newUserId, "John", "Doe",
    //         "jd@example.com", "A new user", "");
    //     u.DeleteUser(createdBy);
    //     Assert.Equal(u.DomainEvents.Last().Timestamp, u.DeletedAt);
    //     Assert.Equal(createdBy, u.DeletedBy);
    // }
    //
    // [Fact]
    // public void Test_0006_User_Undelete()
    // {
    //     var createdBy = Guid.NewGuid();
    //     var newUserId = Guid.NewGuid();
    //     var u = User.Initialize(createdBy, newUserId, "John", "Doe",
    //         "jd@example.com", "A new user", "");
    //     u.DeleteUser(createdBy);
    //     u.UndeleteUser(createdBy);
    //     Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
    //     Assert.Equal(createdBy, u.ModifiedBy);
    // }
}