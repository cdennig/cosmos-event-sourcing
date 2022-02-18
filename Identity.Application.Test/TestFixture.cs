using System;
using System.Threading;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Repository;
using Identity.Domain;

namespace Identity.Application.Test;

public class TestFixture : IDisposable
{
    public IEventsRepository<User, Guid, Guid> CurrentRepo;
    public Guid CurrentCreatedBy { get; set; }
    public Guid CurrentValidUserId { get; set; }
    public Guid CurrentInValidUserId { get; set; }
    public Guid CurrentDeletedUserId { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");

    public TestFixture()
    {
        CurrentRepo = new InMemoryEventsRepository<User, Guid, Guid>(new AggregateRootFactory<User, Guid, Guid>());
        CurrentValidUserId = Guid.NewGuid();
        CurrentCreatedBy = Guid.NewGuid();
        CurrentInValidUserId = Guid.NewGuid();
        CurrentDeletedUserId = Guid.NewGuid();
        var vUser = User.Initialize(CurrentCreatedBy, CurrentValidUserId, "Valid", "Valid", "vv@test.com",
            "Valid user");
        vUser.ConfirmUser(CurrentCreatedBy);
        CurrentRepo.AppendAsync(vUser, CancellationToken.None).GetAwaiter().GetResult();
        var invUser = User.Initialize(CurrentCreatedBy, CurrentInValidUserId, "Invalid", "Invalid", "inv@test.com",
            "Invalid user");
        CurrentRepo.AppendAsync(invUser, CancellationToken.None).GetAwaiter().GetResult();
        var delUser = User.Initialize(CurrentCreatedBy, CurrentDeletedUserId, "Deleted", "Deleted", "del@test.com",
            "Deleted user");
        delUser.DeleteUser(CurrentCreatedBy);
        CurrentRepo.AppendAsync(delUser, CancellationToken.None).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
    }
}