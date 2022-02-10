using System;
using System.Threading.Tasks;
using Identity.Domain;
using Projects.Domain;
using Xunit;

namespace ES.Infrastructure.Test;

[TestCaseOrderer("ES.Infrastructure.Test.AlphabeticalOrderer", "ES.Infrastructure.Test")]
public class InfrastructureTest : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public InfrastructureTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    async Task Test_0001_Append_InMemory()
    {
        var p = Project.Initialize(_fixture.TenantId, _fixture.UserId, _fixture.CurrentId,
            "Test Project", "New Description",
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddMonths(1), ProjectPriority.High);
        p.SetDescriptions(_fixture.UserId, "New Title", "New description");
        p.SetDates(_fixture.UserId, DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
        p.StartProject(_fixture.UserId);
        p.PauseProject(_fixture.UserId);
        p.ResumeProject(_fixture.UserId);
        await _fixture.CurrentTenantRepo.AppendAsync(p);
    }

    [Fact]
    async void Test_0002_Rehydrate_InMemory()
    {
        var p = await _fixture.CurrentTenantRepo.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
        Assert.Equal(ProjectPriority.High, p.Priority);
        Assert.Equal(_fixture.UserId, p.CreatedBy);
        Assert.Equal(_fixture.UserId, p.ModifiedBy);
    }
    
    [Fact]
    async Task Test_0003_Append_InMemory()
    {
        var tenant = Tenant.Initialize(_fixture.UserId, _fixture.TenantId,
            "Test", "", "de-DE", "");
        
        tenant.UpdateGeneralInformation(_fixture.UserId, "New Name", "New description", "");
        await _fixture.CurrentRepo.AppendAsync(tenant);
    }
    
    [Fact]
    async void Test_0004_Rehydrate_InMemory()
    {
        var p = await _fixture.CurrentRepo.RehydrateAsync(_fixture.TenantId);
        Assert.Equal("New Name", p.Name);
        Assert.Equal(_fixture.UserId, p.CreatedBy);
        Assert.Equal(_fixture.UserId, p.ModifiedBy);
    }
}