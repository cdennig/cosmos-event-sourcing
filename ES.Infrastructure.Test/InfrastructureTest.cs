using System;
using System.Threading.Tasks;
using ES.Infrastructure.Repository;
using Projects.Domain;
using Xunit;

namespace ES.Infrastructure.Test
{
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
            var p = Project.Initialize(_fixture.TenantId, _fixture.CurrentId,
                "Test Project", "New Description",
                DateTimeOffset.Now,
                DateTimeOffset.Now.AddMonths(1), ProjectPriority.High);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            await _fixture.CurrentRepo.AppendAsync(p);
        }

        [Fact]
        async void Test_0002_Rehydrate_InMemory()
        {
            var p = await _fixture.CurrentRepo.RehydrateAsync(_fixture.TenantId, _fixture.CurrentId);
            Assert.Equal(ProjectPriority.High, p.Priority);
        }
    }
}