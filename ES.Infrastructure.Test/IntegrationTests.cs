﻿using System;
using Projects.Domain;
using ES.Infrastructure.Repository;
using Xunit;

namespace ES.Infrastructure.Test;

[TestCaseOrderer("ES.Infrastructure.Test.AlphabeticalOrderer", "ES.Infrastructure.Test")]
public class IntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public IntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }
    //
    // [Fact]
    // public async void Test_001_Hydrate()
    // {
    //     var cer = new CosmosEventsRepository<Guid, Project, Guid, Guid>(_fixture.Container);
    //     var p = Project.Initialize(_fixture.TenantId,
    //         _fixture.UserId,
    //         _fixture.CurrentId,
    //         "Test Project",
    //         "Project description",
    //         DateTimeOffset.UtcNow,
    //         DateTimeOffset.Now.AddMonths(3),
    //         ProjectPriority.High);
    //     p.SetDescriptions(_fixture.UserId, "New Title", "New description");
    //     p.SetDates(_fixture.UserId, DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
    //     p.StartProject(_fixture.UserId);
    //     p.PauseProject(_fixture.UserId);
    //     p.ResumeProject(_fixture.UserId);
    //     await cer.AppendAsync(p);
    // }

    [Fact]
    public async void Test_002_Rehydrate()
    {
        var cer = _fixture.Repository;
        var p = await cer.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
    }

    [Fact]
    public async void Test_003_AppendToExistingProject()
    {
        var cer = _fixture.Repository;
        var p = await cer.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
        p.SetPriority(_fixture.UserId, ProjectPriority.VeryLow);
        await cer.AppendAsync(p);
    }
}