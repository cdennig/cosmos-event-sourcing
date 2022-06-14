using Tasks.Application.Commands;
using Tasks.Domain;
using Xunit;

namespace Tasks.Application.Test;

[TestCaseOrderer("Tasks.Application.Test.AlphabeticalOrderer", "Tasks.Application.Test")]
public class ApplicationTest : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public ApplicationTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateTaskAsync()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Description = "Test Task",
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddMonths(3),
            Priority = TaskPriority.VeryHigh,
            Title = "Title of Task",
            TimeEstimation = 2600
        });

        return res.Id;
    }

    private async Task<Domain.Task> ReadTaskFromRepo(Guid tenantId, Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(tenantId, id);
    }

    [Fact]
    async void Test_0001_Create_Task()
    {
        var res = await _fixture.CurrentMediator.Send(new CreateTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Description = "Test Task",
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddMonths(3),
            Priority = TaskPriority.VeryHigh,
            Title = "Title of Task",
            TimeEstimation = 2600
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(1, res.Version);
        Assert.NotEmpty(res.ResourceId);
    }

    [Fact]
    async void Test_0002_Task_SetDescriptions()
    {
        var resCreate = await CreateTaskAsync();

        var res = await _fixture.CurrentMediator.Send(new SetDescriptionsTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Description = "New Description",
            Title = "New Title"
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadTaskFromRepo(res.TenantId, res.Id);
        Assert.Equal("New Description", p.Description);
        Assert.Equal("New Title", p.Title);
    }

    [Fact]
    async void Test_0003_Task_SetDates()
    {
        var resCreate = await CreateTaskAsync();
        var newStart = DateTimeOffset.Now.AddYears(-1);
        var newEnd = DateTimeOffset.Now.AddYears(10);
        var res = await _fixture.CurrentMediator.Send(new SetDatesTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            StartDate = newStart,
            EndDate = newEnd,
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadTaskFromRepo(res.TenantId, res.Id);
        Assert.Equal(newStart, p.StartDate);
        Assert.Equal(newEnd, p.EndDate);
    }

    [Fact]
    async void Test_0004_Task_SetPriority()
    {
        var resCreate = await CreateTaskAsync();

        var res = await _fixture.CurrentMediator.Send(new SetPriorityTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Priority = TaskPriority.VeryLow
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadTaskFromRepo(res.TenantId, res.Id);
        Assert.Equal(TaskPriority.VeryLow, p.Priority);
    }

    [Fact]
    async void Test_0005_Task_Delete()
    {
        var resCreate = await CreateTaskAsync();

        var res = await _fixture.CurrentMediator.Send(new DeleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadTaskFromRepo(res.TenantId, res.Id);
        Assert.True(p.Deleted);
    }

    [Fact]
    async void Test_0006_Task_Undelete()
    {
        var resCreate = await CreateTaskAsync();

        await _fixture.CurrentMediator.Send(new DeleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var res = await _fixture.CurrentMediator.Send(new UndeleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(3, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadTaskFromRepo(res.TenantId, res.Id);
        Assert.False(p.Deleted);
    }

    [Fact]
    async void Test_0007_Task_LogTime()
    {
        var resCreate = await CreateTaskAsync();
        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 120
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(1, p.TimeLogEntries.Count);
        Assert.Equal("First time log entry", p.TimeLogEntries.First().Comment);
        Assert.Equal(day, p.TimeLogEntries.First().Day);
        Assert.Equal(120UL, p.TimeLogEntries.First().Duration);
    }

    [Fact]
    async void Test_0008_Task_Change_TimeLogEntry_Comment()
    {
        var resCreate = await CreateTaskAsync();
        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 120
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(1, p.TimeLogEntries.Count);
        Assert.Equal("First time log entry", p.TimeLogEntries.First().Comment);
        Assert.Equal(day, p.TimeLogEntries.First().Day);
        Assert.Equal(120UL, p.TimeLogEntries.First().Duration);

        await _fixture.CurrentMediator.Send(new ChangeTimeLogEntryCommentCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            EntryId = p.TimeLogEntries.First().Id,
            Comment = "Changed!"
        });

        p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(1, p.TimeLogEntries.Count);
        Assert.Equal("Changed!", p.TimeLogEntries.First().Comment);
    }


    [Fact]
    async void Test_0009_Task_Delete_TimeLogEntry()
    {
        var resCreate = await CreateTaskAsync();
        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 120
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(1, p.TimeLogEntries.Count);
        Assert.Equal("First time log entry", p.TimeLogEntries.First().Comment);
        Assert.Equal(day, p.TimeLogEntries.First().Day);
        Assert.Equal(120UL, p.TimeLogEntries.First().Duration);

        await _fixture.CurrentMediator.Send(new DeleteTimeLogEntryCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            EntryId = p.TimeLogEntries.First().Id
        });

        p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(0, p.TimeLogEntries.Count);
    }

    [Fact]
    async void Test_0010_Task_Set_Complete()
    {
        var resCreate = await CreateTaskAsync();

        await _fixture.CurrentMediator.Send(new SetCompleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.True(p.Completed);
    }

    [Fact]
    async void Test_0011_Task_Set_Incomplete()
    {
        var resCreate = await CreateTaskAsync();

        await _fixture.CurrentMediator.Send(new SetCompleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.True(p.Completed);

        await _fixture.CurrentMediator.Send(new SetIncompleteTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.False(p.Completed);
    }

    [Fact]
    async void Test_0012_Task_Set_Dates()
    {
        var resCreate = await CreateTaskAsync();
        var start = DateTimeOffset.Now;
        await _fixture.CurrentMediator.Send(new SetDatesTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            StartDate = start,
            EndDate = start.AddMonths(1)
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(start, p.StartDate);
        Assert.Equal(start.AddMonths(1), p.EndDate);
    }

    [Fact]
    async void Test_0013_Task_Set_Descriptions()
    {
        var resCreate = await CreateTaskAsync();
        var title = "New Title";
        var description = "New Description";
        await _fixture.CurrentMediator.Send(new SetDescriptionsTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Description = description,
            Title = title
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(title, p.Title);
        Assert.Equal(description, p.Description);
    }

    [Fact]
    async void Test_0014_Task_Set_TimeEstimation()
    {
        var resCreate = await CreateTaskAsync();
        var te = 140UL;

        await _fixture.CurrentMediator.Send(new SetTimeEstimationTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Estimation = te
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(te, p.TimeEstimation);
    }

    [Fact]
    async void Test_0015_Task_TimeRemaining()
    {
        var resCreate = await CreateTaskAsync();
        var te = 140UL;

        await _fixture.CurrentMediator.Send(new SetTimeEstimationTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Estimation = te
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(te, p.TimeEstimation);

        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 120
        });

        p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(20L, p.TimeRemaining);
    }

    [Fact]
    async void Test_0016_Task_TimeRemaining_Negative()
    {
        var resCreate = await CreateTaskAsync();
        var te = 140UL;

        await _fixture.CurrentMediator.Send(new SetTimeEstimationTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Estimation = te
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(te, p.TimeEstimation);

        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 160
        });

        p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(-20L, p.TimeRemaining);
    }
    
    
    [Fact]
    async void Test_0017_Task_TimeRemaining_NoEstimation()
    {
        var resCreate = await CreateTaskAsync();
        var day = DateOnly.FromDateTime(DateTime.Now);

        await _fixture.CurrentMediator.Send(new LogTimeTaskCommand()
        {
            PrincipalId = _fixture.PrincipalId,
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Comment = "First time log entry",
            Day = day,
            Duration = 160
        });

        var p = await ReadTaskFromRepo(_fixture.TenantId, resCreate);
        Assert.Equal(-160L, p.TimeRemaining);
    }
}