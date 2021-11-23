using System;
using ES.Shared.Aggregate;
using Xunit;

namespace Projects.Domain.Test
{
    [TestCaseOrderer("Projects.Domain.Test.AlphabeticalOrderer", "Projects.Domain.Test")]
    public class ProjectTest
    {
        [Fact]
        public void Test_0001_StartProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            Assert.NotNull(p.ModifiedAt);
            Assert.NotNull(p.StartDate);
            Assert.NotNull(p.ActualStartDate);
            Assert.Equal(p.StartDate, p.ActualStartDate);
            Assert.Equal(ProjectStatus.Started, p.Status);
        }

        [Fact]
        public void Test_0002_PauseProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.PauseProject();
            Assert.Equal(ProjectStatus.Paused, p.Status);
        }

        [Fact]
        public void Test_0003_PauseProjectThrowsErrorWhenInvalidStatus()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.PauseProject();
            Assert.Throws<ArgumentException>(Action);
            p.StartProject();
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0004_ResumeProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            Assert.Equal(ProjectStatus.Resumed, p.Status);
        }


        [Fact]
        public void Test_0005_ResumeProjectThrowsErrorWhenInvalidStatus()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.ResumeProject();
            Assert.Throws<ArgumentException>(Action);
            p.StartProject();
            Assert.Throws<ArgumentException>(Action);
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0006_CancelProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.CancelProject();
            Assert.Equal(ProjectStatus.Cancelled, p.Status);
        }


        [Fact]
        public void Test_0007_CancelProjectThrowsErrorWhenInvalidStatus()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.CancelProject();
            void Action() => p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }


        [Fact]
        public void Test_0008_FinishProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.FinishProject();
            Assert.Equal(ProjectStatus.Finished, p.Status);
        }


        [Fact]
        public void Test_0009_FinishProjectThrowsErrorWhenInvalidStatus()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.FinishProject();
            Assert.Throws<ArgumentException>(Action);
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0010_RecreateProject()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
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
        public void Test_0011_UpdatePriority()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.SetPriority(ProjectPriority.VeryHigh);
            Assert.Equal(ProjectPriority.VeryHigh, p.Priority);
        }

        [Fact]
        public void Test_0012_ProjectReadOnly()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.FinishProject();
            void Action() => p.SetDescriptions("Should throw", "an exception");
            Assert.Throws<Exception>(Action);
        }

        [Fact]
        public void Test_0013_ProjectDeleteAndUndelete()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.DeleteProject();
            Assert.True(p.Deleted);
            Assert.NotNull(p.DeletedAt);
            p.UndeleteProject();
            Assert.False(p.Deleted);
        }

        [Fact]
        public void Test_0014_ProjectHasResourceId()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            Assert.Equal($"/orgs/{tenantId}/projects/{projectId}", p.ResourceId);
        }
    }
}