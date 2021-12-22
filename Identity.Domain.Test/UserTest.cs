using System;
using System.Linq;
using Xunit;

namespace Identity.Domain.Test;

[TestCaseOrderer("Identity.Domain.Test.AlphabeticalOrderer", "Identity.Domain.Test")]
public class UserTest
{
    [Fact]
    public void Test_0001_User_Create()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.CreatedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.CreatedBy);
        Assert.Equal("John", u.FirstName);
        Assert.Equal("Doe", u.LastName);
        Assert.Equal("jd@example.com", u.Email);
        Assert.Equal("A new user", u.Description);
        Assert.Equal(UserStatus.ConfirmationRequested, u.Status);
        Assert.Empty(u.PictureUri);
    }

    [Fact]
    public void Test_0002_User_Confirm()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        u.ConfirmUser(createdBy);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(u.DomainEvents.Last().RaisedBy, u.ModifiedBy);
        Assert.Equal(UserStatus.Confirmed, u.Status);
    }

    [Fact]
    public void Test_0003_User_Update_PersonalInformation()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        u.UpdatePersonalInformation(createdBy, "Jane", "Bar", "Another description", "/uri/123.png");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(createdBy, u.ModifiedBy);
        Assert.Equal("Jane", u.FirstName);
        Assert.Equal("Bar", u.LastName);
        Assert.Equal("Another description", u.Description);
        Assert.Equal("/uri/123.png", u.PictureUri);
    }
    
    [Fact]
    public void Test_0004_User_Update_Email()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        u.UpdateEmail(createdBy, "john.doe@company.com");
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(createdBy, u.ModifiedBy);
        Assert.Equal("john.doe@company.com", u.Email);
    }    
    
    [Fact]
    public void Test_0004_User_Delete()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        u.DeleteUser(createdBy);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.DeletedAt);
        Assert.Equal(createdBy, u.DeletedBy);
    }    
    
    [Fact]
    public void Test_0004_User_Undelete()
    {
        var createdBy = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var u = User.Initialize(createdBy, newUserId, "John", "Doe",
            "jd@example.com", "A new user", "");
        u.DeleteUser(createdBy);
        u.UndeleteUser(createdBy);
        Assert.Equal(u.DomainEvents.Last().Timestamp, u.ModifiedAt);
        Assert.Equal(createdBy, u.ModifiedBy);
    }
}