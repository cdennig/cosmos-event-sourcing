using System;
using Projects.Domain;
using ES.Infrastructure.Repository;
using Xunit;

namespace ES.Infrastructure.Test
{
    [TestCaseOrderer("ES.Infrastructure.Test.AlphabeticalOrderer", "ES.Infrastructure.Test")]
    public class IntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public IntegrationTests(TestFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public async void Test_001_Hydrate()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
            var p = Project.Initialize(_fixture.TenantId, _fixture.CurrentId,"Test Project", DateTimeOffset.UtcNow);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            await cer.AppendAsync(p);
        }
        
        [Fact]
        public async void Test_002_Rehydrate()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(_fixture.TenantId,_fixture.CurrentId);
        }
        
        
    }
}