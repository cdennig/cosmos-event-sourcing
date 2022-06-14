using System;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using Identity.Domain;
using Microsoft.Extensions.Logging;
using Projects.Domain;

namespace ES.Infrastructure.Test;

public class TestFixture : IDisposable
{
    public InMemoryTenantEventsRepository<Guid, Project, Guid, Guid> CurrentTenantRepo;

    public InMemoryEventsRepository<Tenant, Guid, Guid> CurrentRepo;

    public Guid CurrentId => Guid.Parse("cb996bc3-f663-4042-9a6a-a6a410c32938");
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid UserId => Guid.Parse("c576f6ce-ee4f-4d04-a9d3-63c590d322f1");

    public TestFixture()
    {
        var loggerFactory = (ILoggerFactory)new LoggerFactory();
        var tenantlogger = loggerFactory.CreateLogger<InMemoryTenantEventsRepository<Guid, Project, Guid, Guid>>();
        var logger = loggerFactory.CreateLogger<InMemoryEventsRepository<Tenant, Guid, Guid>>();
        var tarfLogger = loggerFactory.CreateLogger<TenantAggregateRootFactory<Guid, Project, Guid, Guid>>();
        var arfLogger = loggerFactory.CreateLogger<AggregateRootFactory<Tenant, Guid, Guid>>();
        CurrentRepo =
            new(new AggregateRootFactory<Tenant, Guid, Guid>(arfLogger), logger);
        CurrentTenantRepo =
            new(new TenantAggregateRootFactory<Guid, Project, Guid, Guid>(tarfLogger), tenantlogger);
    }

    public void Dispose()
    {
    }
}