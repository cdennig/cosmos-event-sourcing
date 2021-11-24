using System;
using System.Linq;
using ES.Shared.Aggregate;
using Xunit;

namespace Tasks.Domain.Test
{
    [TestCaseOrderer("Tasks.Domain.Test.AlphabeticalOrderer", "Tasks.Domain.Test")]
    public class TaskTest
    {
        [Fact]
        public void Test_0001_Init_Task()
        {
            var task = Task.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Task", "Test Description", null,
                DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(3), TaskPriority.VeryHigh);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.CreatedAt);
            Assert.False(task.Completed);
        }

        [Fact]
        public void Test_0002_Task_UpdateDescriptions()
        {
            var task = Task.Initialize(Guid.NewGuid(), Guid.NewGuid(), "Test Task", "Test Description", null,
                DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(3), TaskPriority.VeryHigh);
            task.SetDescriptions("Update", "Update");
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
            Assert.Equal("Update", task.Description);
            Assert.Equal("Update", task.Title);
        }

        [Fact]
        public void Test_0003_Task_SetDates()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            var startDate = DateTimeOffset.Now;
            var endDate = startDate.AddMonths(1);
            task.SetDates(startDate, endDate);
            Assert.Equal(startDate, task.StartDate);
            Assert.Equal(endDate, task.EndDate);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        }

        [Fact]
        public void Test_0004_Task_SetPriority()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            task.SetPriority(TaskPriority.VeryLow);
            Assert.Equal(TaskPriority.VeryLow, task.Priority);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        }

        [Fact]
        public void Test_0005_Task_Delete_And_Undelete()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            task.DeleteTask();
            Assert.True(task.Deleted);
            Assert.NotNull(task.DeletedAt);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.DeletedAt);
            task.UndeleteTask();
            Assert.False(task.Deleted);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        }

        [Fact]
        public void Test_0006_Task_SetComplete_And_SetIncomplete()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            task.SetComplete();
            Assert.True(task.Completed);
            Assert.NotNull(task.CompletedAt);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.CompletedAt);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
            task.SetIncomplete();
            Assert.False(task.Completed);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
            Assert.Null(task.CompletedAt);
        }

        [Fact]
        public void Test_0007_Task_AssignToProject()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            task.AssignToProject(projectId);
            Assert.Equal(projectId, task.ProjectId);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        }

        [Fact]
        public void Test_0008_Task_RemoveFromProject()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task");
            task.AssignToProject(projectId);
            task.RemoveFromProject();
            Assert.Null(task.ProjectId);
            Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        }

        [Fact]
        public void Test_0008_Task_Has_Project_ResourceId()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task", "", projectId);
            Assert.Equal($"/orgs/{tenantId}/projects/{projectId}/tasks/{taskId}", task.ResourceId);
            task.RemoveFromProject();
            Assert.Equal($"/orgs/{tenantId}/tasks/{taskId}", task.ResourceId);
        }

        [Fact]
        public void Test_0009_Task_LogTime()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task", "", projectId);
            var day = DateOnly.FromDateTime(DateTime.Now);
            task.LogTime(180, "First time log entry", day);
            Assert.Equal(1, task.TimeLogEntries.Count);
            var tle = task.TimeLogEntries.First();
            Assert.Equal(task, tle.Parent);
            Assert.Equal("First time log entry", tle.Comment);
            Assert.Equal(180UL, tle.Duration);
            Assert.Equal(day, tle.Day);
        }

        [Fact]
        public void Test_0010_Task_Recreate()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task", "", projectId);
            task.SetTimeEstimation(960);
            var day = DateOnly.FromDateTime(DateTime.Now);
            task.LogTime(180, "First time log entry", day);
            var newTask = BaseAggregateRoot<Guid, Task, Guid>.Create(tenantId, taskId, task.DomainEvents);
            Assert.Equal(task.TimeEstimation, newTask.TimeEstimation);
            Assert.Equal(newTask.TimeLogEntries.Count, task.TimeLogEntries.Count);
            var tle = newTask.TimeLogEntries.First();
            Assert.Equal(task.Id, tle.Parent.Id);
            Assert.Equal(task.TimeLogEntries.First().Comment, tle.Comment);
            Assert.Equal(task.TimeLogEntries.First().Duration, tle.Duration);
            Assert.Equal(task.TimeLogEntries.First().Day, tle.Day);
        }
        
        [Fact]
        public void Test_0011_Task_LogTime_And_Remove()
        {
            var tenantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var task = Task.Initialize(tenantId, taskId, "Test Task", "", projectId);
            var day = DateOnly.FromDateTime(DateTime.Now);
            task.LogTime(180, "First time log entry", day);
            Assert.Equal(1, task.TimeLogEntries.Count);
            task.DeleteTimeLogEntry(task.TimeLogEntries.First().Id);
            Assert.Equal(0, task.TimeLogEntries.Count);
        }
    }
}