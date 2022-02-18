using System;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using Identity.Domain;
using Projects.Domain;

namespace ES.Infrastructure.Test;

public class TestFixture : IDisposable
{
    public InMemoryTenantEventsRepository<Guid, Project, Guid, Guid> CurrentTenantRepo =
        new(new TenantAggregateRootFactory<Guid, Project, Guid, Guid>());

    public InMemoryEventsRepository<Tenant, Guid, Guid> CurrentRepo =
        new(new AggregateRootFactory<Tenant, Guid, Guid>());

    public Guid CurrentId => Guid.Parse("cb996bc3-f663-4042-9a6a-a6a410c32938");
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid UserId => Guid.Parse("c576f6ce-ee4f-4d04-a9d3-63c590d322f1");

    public void Dispose()
    {
    }
}