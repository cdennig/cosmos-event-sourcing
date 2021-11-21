using System;
using Projects.Domain;
using Projects.Infrastructure.Repository;
using Xunit;

namespace Projects.Infrastructure.Test
{
    public class IntegrationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public IntegrationTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        // [Fact]
        // public async void TestHydrate()
        // {
        //     var cer = new CosmosEventsRepository<Project, Guid>(_fixture.Container);
        //     var p = Project.Initialize(Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
        //     p.SetDescriptions("New Title", "New description");
        //     p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
        //     p.StartProject();
        //     p.PauseProject();
        //     p.ResumeProject();
        //     p.FinishProject();
        //     await cer.AppendAsync(p);
        // }

        [Fact]
        public async void TestRehydrate()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"),
                Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"));
            p.SetDescriptions("My new Title", "My new Description");
            await cer.AppendAsync(p);
        }

        [Fact]
        public async void TestRehydrate2()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"),
                Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"));
        }

        [Fact]
        public async void TestRehydrate3()
        {
            var cer = new CosmosEventsRepository<Guid, Project, Guid>(_fixture.Container);
            var p = await cer.RehydrateAsync(Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"),
                Guid.Parse("abd9e633-9b35-4e5b-a47b-8db8c6542b4b"));
        }
    }
}