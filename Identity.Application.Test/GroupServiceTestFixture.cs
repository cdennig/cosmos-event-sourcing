using System;
using System.Threading;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Repository;
using Identity.Domain;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Test;

public class GroupServiceTestFixture : IDisposable
{
    public ITenantEventsRepository<Guid, Group, Guid, Guid> CurrentRepo;
    public Guid CurrentCreatedBy { get; set; }
    public Guid CurrentValidGroupId { get; set; }
    public Guid CurrentDeletedGroupId { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");

    public GroupServiceTestFixture()
    {
        var loggerFactory = (ILoggerFactory)new LoggerFactory();
        var logger = loggerFactory.CreateLogger<InMemoryTenantEventsRepository<Guid, Group, Guid, Guid>>();
        var tarfLogger = loggerFactory.CreateLogger<TenantAggregateRootFactory<Guid, Group, Guid, Guid>>();
        CurrentRepo =
            new InMemoryTenantEventsRepository<Guid, Group, Guid, Guid>(
                new TenantAggregateRootFactory<Guid, Group, Guid, Guid>(tarfLogger), logger);

        CurrentValidGroupId = Guid.NewGuid();
        CurrentCreatedBy = Guid.NewGuid();

        CurrentDeletedGroupId = Guid.NewGuid();
        var vGroup = Group.Initialize(TenantId, CurrentCreatedBy, CurrentValidGroupId, "Group Valid", "Valid", "");
        CurrentRepo.AppendAsync(vGroup, CancellationToken.None).GetAwaiter().GetResult();
        var delGroup = Group.Initialize(TenantId, CurrentCreatedBy, CurrentDeletedGroupId, "Group Deleted", "Deleted",
            "");
        delGroup.DeleteGroup(CurrentCreatedBy);
        CurrentRepo.AppendAsync(delGroup, CancellationToken.None).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
    }
}