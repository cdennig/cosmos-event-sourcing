using System;
using Projects.Application.Commands;
using Projects.Domain;
using Xunit;

namespace Projects.Application.Test
{
    [TestCaseOrderer("Projects.Application.Test.AlphabeticalOrderer", "Projects.Application.Test")]
    public class ApplicationTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public ApplicationTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        async void Test_0001_Create_Project()
        {
            var res = await _fixture.CurrentMediator.Send(new CreateProjectCommand()
            {
                TenantId = _fixture.TenantId,
                Description = "Test Project",
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddMonths(3),
                Priority = ProjectPriority.VeryHigh,
                Title = "Title of Project"
            });
            Assert.NotNull(res);
            Assert.NotEqual(Guid.Empty, res.Id);
            Assert.Equal(1, res.Version);
            Assert.NotEmpty(res.ResourceId);
            // Set ID of current project
            _fixture.CurrentId = res.Id;
        }
        
        [Fact]
        async void Test_0002_Start_Project()
        {
            var res = await _fixture.CurrentMediator.Send(new StartProjectCommand()
            {
                TenantId = _fixture.TenantId,
                Id = _fixture.CurrentId
            });
            Assert.NotNull(res);
            Assert.NotEqual(Guid.Empty, res.Id);
            Assert.Equal(2, res.Version);
            Assert.NotEmpty(res.ResourceId);
        }
        
        [Fact]
        async void Test_0003_Pause_Project()
        {
            var res = await _fixture.CurrentMediator.Send(new PauseProjectCommand()
            {
                TenantId = _fixture.TenantId,
                Id = _fixture.CurrentId
            });
            Assert.NotNull(res);
            Assert.NotEqual(Guid.Empty, res.Id);
            Assert.Equal(3, res.Version);
            Assert.NotEmpty(res.ResourceId);
        }
        
        [Fact]
        async void Test_0004_Resume_Project()
        {
            var res = await _fixture.CurrentMediator.Send(new ResumeProjectCommand()
            {
                TenantId = _fixture.TenantId,
                Id = _fixture.CurrentId
            });
            Assert.NotNull(res);
            Assert.NotEqual(Guid.Empty, res.Id);
            Assert.Equal(4, res.Version);
            Assert.NotEmpty(res.ResourceId);
        }
    }
}