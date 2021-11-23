using System;
using System.Linq;
using ES.Shared.Aggregate;
using Xunit;

namespace Projects.Domain.Test
{
    [TestCaseOrderer("Projects.Domain.Test.AlphabeticalOrderer", "Projects.Domain.Test")]
    public class ProjectTest
    {
        [Fact]
        public void Test_0001_Start_Project()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
            Assert.NotNull(p.StartDate);
            Assert.NotNull(p.ActualStartDate);
            Assert.Equal(p.StartDate, p.ActualStartDate);
            Assert.Equal(ProjectStatus.Started, p.Status);
        }

        [Fact]
        public void Test_0002_Start_Project_Throws_Error_When_Invalid_Status()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.StartProject();
            p.StartProject();
            Assert.Throws<ArgumentException>(Action);
            p.PauseProject();
            Assert.Throws<ArgumentException>(Action);
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0003_Pause_Project()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.PauseProject();
            Assert.Equal(ProjectStatus.Paused, p.Status);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }

        [Fact]
        public void Test_0004_Pause_Project_Throws_Error_When_Invalid_Status()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.PauseProject();
            Assert.Throws<ArgumentException>(Action);
            p.StartProject();
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0005_Resume_Project()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            Assert.Equal(ProjectStatus.Resumed, p.Status);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }


        [Fact]
        public void Test_0006_Resume_Project_Throws_Error_When_Invalid_Status()
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
        public void Test_0007_Cancel_Project()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.CancelProject();
            Assert.Equal(ProjectStatus.Cancelled, p.Status);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }


        [Fact]
        public void Test_0008_Cancel_Project_Throws_Error_When_Invalid_Status()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.CancelProject();
            void Action() => p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }


        [Fact]
        public void Test_0009_Finish_Project()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.FinishProject();
            Assert.Equal(ProjectStatus.Finished, p.Status);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
            Assert.NotNull(p.EndDate);
            Assert.NotNull(p.ActualEndDate);
            Assert.Equal(p.EndDate, p.ActualEndDate);
        }


        [Fact]
        public void Test_0010_Finish_Project_Throws_Error_When_Invalid_Status()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            void Action() => p.FinishProject();
            Assert.Throws<ArgumentException>(Action);
            p.CancelProject();
            Assert.Throws<ArgumentException>(Action);
        }

        [Fact]
        public void Test_0011_Recreate_Project()
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
        public void Test_0012_Update_Priority()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.SetPriority(ProjectPriority.VeryHigh);
            Assert.Equal(ProjectPriority.VeryHigh, p.Priority);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }

        [Fact]
        public void Test_0013_Project_ReadOnly()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.StartProject();
            p.FinishProject();
            void Action() => p.SetDescriptions("Should throw", "an exception");
            Assert.Throws<Exception>(Action);
        }

        [Fact]
        public void Test_0014_Project_Delete_And_Undelete()
        {
            var p = Project.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Project");
            p.DeleteProject();
            Assert.True(p.Deleted);
            Assert.NotNull(p.DeletedAt);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.DeletedAt);
            p.UndeleteProject();
            Assert.False(p.Deleted);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }

        [Fact]
        public void Test_0015_Project_Has_ResourceId()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            Assert.Equal($"/orgs/{tenantId}/projects/{projectId}", p.ResourceId);
        }

        [Fact]
        public void Test_0016_Project_SetDescriptions()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            var newTitle = "New Title";
            var newDescription = "New Description";
            p.SetDescriptions(newTitle, newDescription);
            Assert.Equal(newTitle, p.Title);
            Assert.Equal(newDescription, p.Description);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }

        [Fact]
        public void Test_0017_Project_SetDates()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            var startDate = DateTimeOffset.Now;
            var endDate = startDate.AddYears(1);
            p.SetDates(startDate, endDate);
            Assert.Equal(startDate, p.StartDate);
            Assert.Equal(endDate, p.EndDate);
            Assert.Equal(p.DomainEvents.Last().Timestamp, p.ModifiedAt);
        }

        [Fact]
        public void Test_0018_Project_SetDescriptions_Throws_Error_When_Empty_Or_ReadOnly()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            var newTitle = "";
            var newDescription = "";
            void Action() => p.SetDescriptions(newTitle, newDescription);
            Assert.Throws<ArgumentException>(Action);
            p.CancelProject();
            newTitle = "New Title";
            Assert.Throws<Exception>(Action);
        }

        [Fact]
        public void Test_0019_Project_SetPriority_Throws_When_ReadOnly()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            p.DeleteProject();
            void Action() => p.SetPriority(ProjectPriority.VeryLow);
            Assert.Throws<Exception>(Action);
        }

        [Fact]
        public void Test_0020_Project_SetDates_Throws_When_Invalid_Dates_Or_ReadOnly()
        {
            var tenantId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var p = Project.Initialize(tenantId, projectId, "Test Project");
            var endDate = DateTimeOffset.Now;
            var startDate = endDate.AddYears(1);
            void Action() => p.SetDates(startDate, endDate);
            Assert.Throws<ArgumentException>(Action);
            p.DeleteProject();
            void Action2() => p.SetDates(endDate, startDate);
            Assert.Throws<Exception>(Action2);
        }
    }
}