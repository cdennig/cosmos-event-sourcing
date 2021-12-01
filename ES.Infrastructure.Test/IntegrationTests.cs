using System;
using Projects.Domain;
using ES.Infrastructure.Repository;
using Xunit;

namespace ES.Infrastructure.Test
{
    [TestCaseOrderer("ES.Infrastructure.Test.AlphabeticalOrderer", "ES.Infrastructure.Test")]
    public class IntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;

        public IntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        // [Fact]
        // public async void Test_001_Hydrate()
        // {
        //     var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
        //     var p = Project.Initialize(_fixture.TenantId, _fixture.CurrentId,"Test Project", DateTimeOffset.UtcNow);
        //     p.SetDescriptions("New Title", "New description");
        //     p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
        //     p.StartProject();
        //     p.PauseProject();
        //     p.ResumeProject();
        //     await cer.AppendAsync(p);
        // }

        [Fact]
        public async void Test_002_Rehydrate()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
        }

        [Fact]
        public async void Test_003_AppendToExistingProject()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
            p.SetPriority(_fixture.UserId, ProjectPriority.High);
            await cer.AppendAsync(p);
        }
    }
}