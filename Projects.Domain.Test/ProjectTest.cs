using System;
using ES.Shared.Aggregate;
using Xunit;

namespace Projects.Domain.Test
{
    [TestCaseOrderer("Projects.Domain.Test.AlphabeticalOrderer", "Projects.Domain.Test")]
    public class ProjectTest
    {
        [Fact]
        public void Test_0001_NormalWorkflow()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            p.FinishProject();
        }

        [Fact]
        public void Test_0002_CancelWorkflow()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.StartProject();
            p.CancelProject();
        }

        [Fact]
        public void Test_0003_Recreate()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            p.FinishProject();
            var pNew = BaseAggregateRoot<Guid, Project, Guid>.Create(p.TenantId, p.Id, p.DomainEvents);
            Assert.Equal(p.Status, pNew.Status);
            Assert.Equal(p.Description, pNew.Description);
            Assert.Equal(p.Priority, pNew.Priority);
            Assert.Equal(p.Title, pNew.Title);
            Assert.Equal(p.EndDate, pNew.EndDate);
            Assert.Equal(p.StartDate, pNew.StartDate);
            Assert.Equal(p.Deleted, pNew.Deleted);
        }

        [Fact]
        public void Test_0004_Priority()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetPriority(ProjectPriority.VeryHigh);
            Assert.Equal(ProjectPriority.VeryHigh, p.Priority);
        }

        [Fact]
        public void Test_0005_ProjectReadOnly()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.StartProject();
            p.FinishProject();
            void Action() => p.SetDescriptions("Should throw", "an exception");
            Assert.Throws<Exception>(Action);
        }
        
        [Fact]
        public void Test_0006_ProjectDeleteAndUndelete()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.DeleteProject();
            Assert.True(p.Deleted);
            Assert.NotNull(p.DeletedAt);
            p.UndeleteProject();
            Assert.False(p.Deleted);
        }
    }
}