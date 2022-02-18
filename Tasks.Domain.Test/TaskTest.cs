using System;
using System.Linq;
using ES.Shared.Aggregate;
using Xunit;

namespace Tasks.Domain.Test;

[TestCaseOrderer("Tasks.Domain.Test.AlphabeticalOrderer", "Tasks.Domain.Test")]
public class TaskTest
{
    [Fact]
    public void Test_0001_Init_Task()
    {
        var user = Guid.NewGuid();
        var task = Task.Initialize(Guid.NewGuid(), user, Guid.NewGuid(), "Test Task", "Test Description", null,
            DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(3), TaskPriority.VeryHigh);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.CreatedAt);
        Assert.False(task.Completed);
    }

    [Fact]
    public void Test_0002_Task_UpdateDescriptions()
    {
        var user = Guid.NewGuid();
        var task = Task.Initialize(Guid.NewGuid(), user, Guid.NewGuid(), "Test Task", "Test Description", null,
            DateTimeOffset.Now, DateTimeOffset.Now.AddMonths(3), TaskPriority.VeryHigh);
        task.SetDescriptions(user, "Update", "Update");
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        Assert.Equal("Update", task.Description);
        Assert.Equal("Update", task.Title);
    }

    [Fact]
    public void Test_0003_Task_SetDates()
    {
        var user = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        var startDate = DateTimeOffset.Now;
        var endDate = startDate.AddMonths(1);
        task.SetDates(user, startDate, endDate);
        Assert.Equal(startDate, task.StartDate);
        Assert.Equal(endDate, task.EndDate);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
    }

    [Fact]
    public void Test_0004_Task_SetPriority()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        task.SetPriority(user, TaskPriority.VeryLow);
        Assert.Equal(TaskPriority.VeryLow, task.Priority);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
    }

    [Fact]
    public void Test_0005_Task_Delete_And_Undelete()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        task.DeleteTask(user);
        Assert.True(task.Deleted);
        Assert.NotNull(task.DeletedAt);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.DeletedAt);
        task.UndeleteTask(user);
        Assert.False(task.Deleted);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
    }

    [Fact]
    public void Test_0006_Task_SetComplete_And_SetIncomplete()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        task.SetComplete(user);
        Assert.True(task.Completed);
        Assert.NotNull(task.CompletedAt);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.CompletedAt);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        task.SetIncomplete(user);
        Assert.False(task.Completed);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void Test_0007_Task_AssignToProject()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        task.AssignToProject(user, projectId);
        Assert.Equal(projectId, task.ProjectId);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
    }

    [Fact]
    public void Test_0008_Task_RemoveFromProject()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task");
        task.AssignToProject(user, projectId);
        task.RemoveFromProject(user);
        Assert.Null(task.ProjectId);
        Assert.Equal(task.DomainEvents.Last().Timestamp, task.ModifiedAt);
    }

    [Fact]
    public void Test_0009_Task_Has_Project_ResourceId()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        Assert.Equal($"/t/{tenantId}/projects/{projectId}/tasks/{taskId}", task.ResourceId);
        task.RemoveFromProject(user);
        Assert.Equal($"/t/{tenantId}/tasks/{taskId}", task.ResourceId);
    }

    [Fact]
    public void Test_0010_Task_LogTime()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        var day = DateOnly.FromDateTime(DateTime.Now);
        task.LogTime(user, 180, "First time log entry", day);
        Assert.Equal(1, task.TimeLogEntries.Count);
        var tle = task.TimeLogEntries.First();
        Assert.Equal(task, tle.Parent);
        Assert.Equal("First time log entry", tle.Comment);
        Assert.Equal(180UL, tle.Duration);
        Assert.Equal(day, tle.Day);
    }

    [Fact]
    public void Test_0011_Task_Recreate()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        task.SetTimeEstimation(user, 960);
        var day = DateOnly.FromDateTime(DateTime.Now);
        task.LogTime(user, 180, "First time log entry", day);
        var factory = new TenantAggregateRootFactory<Guid, Task, Guid, Guid>();
        var newTask = factory.Create(tenantId, taskId, task.DomainEvents);
        Assert.Equal(task.TimeEstimation, newTask.TimeEstimation);
        Assert.Equal(newTask.TimeLogEntries.Count, task.TimeLogEntries.Count);
        var tle = newTask.TimeLogEntries.First();
        Assert.Equal(task.Id, tle.Parent.Id);
        Assert.Equal(task.TimeLogEntries.First().Comment, tle.Comment);
        Assert.Equal(task.TimeLogEntries.First().Duration, tle.Duration);
        Assert.Equal(task.TimeLogEntries.First().Day, tle.Day);
    }

    [Fact]
    public void Test_0012_Task_LogTime_And_Remove()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        var day = DateOnly.FromDateTime(DateTime.Now);
        task.LogTime(user, 180, "First time log entry", day);
        Assert.Equal(1, task.TimeLogEntries.Count);
        task.DeleteTimeLogEntry(user, task.TimeLogEntries.First().Id);
        Assert.Equal(0, task.TimeLogEntries.Count);
    }

    [Fact]
    public void Test_0013_Task_LogTime_Change_Comment()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        var day = DateOnly.FromDateTime(DateTime.Now);
        task.LogTime(user, 180, "First time log entry", day);
        Assert.Equal(1, task.TimeLogEntries.Count);
        task.ChangeTimeLogEntryComment(user, task.TimeLogEntries.First().Id, "New comment");
        Assert.Equal("New comment", task.TimeLogEntries.First().Comment);
    }

    [Fact]
    public void Test_0014_Task_LogTime_Change_Comment_Throws_With_Invalid_Id()
    {
        var tenantId = Guid.NewGuid();
        var user = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = Task.Initialize(tenantId, user, taskId, "Test Task", "", projectId);
        var day = DateOnly.FromDateTime(DateTime.Now);
        task.LogTime(user, 180, "First time log entry", day);
        void Act() => task.ChangeTimeLogEntryComment(user, Guid.NewGuid(), "New comment");
        Assert.Throws<ArgumentException>(Act);
    }
}