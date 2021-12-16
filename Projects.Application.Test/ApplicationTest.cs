using System;
using System.Threading.Tasks;
using Projects.Application.Commands;
using Projects.Domain;
using Xunit;

namespace Projects.Application.Test;

[TestCaseOrderer("Projects.Application.Test.AlphabeticalOrderer", "Projects.Application.Test")]
public class ApplicationTest : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public ApplicationTest(TestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<Guid> CreateProjectAsync()
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

        return res.Id;
    }

    private async Task<Project> ReadProjectFromRepo(Guid tenantId, Guid id)
    {
        return await _fixture.CurrentRepo.RehydrateAsync(tenantId, id);
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
    }

    [Fact]
    async void Test_0002_Start_Project()
    {
        var resCreate = await CreateProjectAsync();

        var res = await _fixture.CurrentMediator.Send(new StartProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectStatus.Started, p.Status);
    }

    [Fact]
    async void Test_0003_Pause_Project()
    {
        var resCreate = await CreateProjectAsync();

        var resStart = await _fixture.CurrentMediator.Send(new StartProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var res = await _fixture.CurrentMediator.Send(new PauseProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(3, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectStatus.Paused, p.Status);
    }

    [Fact]
    async void Test_0004_Resume_Project()
    {
        var resCreate = await CreateProjectAsync();

        var resStart = await _fixture.CurrentMediator.Send(new StartProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var resPause = await _fixture.CurrentMediator.Send(new PauseProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        var res = await _fixture.CurrentMediator.Send(new ResumeProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(4, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectStatus.Resumed, p.Status);
    }

    [Fact]
    async void Test_0005_Finish_Project()
    {
        var resCreate = await CreateProjectAsync();

        var resStart = await _fixture.CurrentMediator.Send(new StartProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var res = await _fixture.CurrentMediator.Send(new FinishProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(3, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectStatus.Finished, p.Status);
    }

    [Fact]
    async void Test_0006_Cancel_Project()
    {
        var resCreate = await CreateProjectAsync();

        var resStart = await _fixture.CurrentMediator.Send(new StartProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });

        var res = await _fixture.CurrentMediator.Send(new CancelProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(3, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectStatus.Cancelled, p.Status);
    }

    [Fact]
    async void Test_0007_Project_SetDescriptions()
    {
        var resCreate = await CreateProjectAsync();

        var res = await _fixture.CurrentMediator.Send(new SetProjectDescriptionsCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Description = "New Description",
            Title = "New Title"
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal("New Description", p.Description);
        Assert.Equal("New Title", p.Title);
    }

    [Fact]
    async void Test_0008_Project_SetDates()
    {
        var resCreate = await CreateProjectAsync();
        var newStart = DateTimeOffset.Now.AddYears(-1);
        var newEnd = DateTimeOffset.Now.AddYears(10);
        var res = await _fixture.CurrentMediator.Send(new SetProjectDatesCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate,
            StartDate = newStart,
            EndDate = newEnd,
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(newStart, p.StartDate);
        Assert.Equal(newEnd, p.EndDate);
    }

    [Fact]
    async void Test_0009_Project_SetPriority()
    {
        var resCreate = await CreateProjectAsync();

        var res = await _fixture.CurrentMediator.Send(new SetProjectPriorityCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate,
            Priority = ProjectPriority.VeryLow
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.Equal(ProjectPriority.VeryLow, p.Priority);
    }

    [Fact]
    async void Test_0010_Project_Delete()
    {
        var resCreate = await CreateProjectAsync();

        var res = await _fixture.CurrentMediator.Send(new DeleteProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(2, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.True(p.Deleted);
    }
        
    [Fact]
    async void Test_0011_Project_Undelete()
    {
        var resCreate = await CreateProjectAsync();

        await _fixture.CurrentMediator.Send(new DeleteProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
            
        var res = await _fixture.CurrentMediator.Send(new UndeleteProjectCommand()
        {
            TenantId = _fixture.TenantId,
            Id = resCreate
        });
        Assert.NotNull(res);
        Assert.NotEqual(Guid.Empty, res.Id);
        Assert.Equal(3, res.Version);
        Assert.NotEmpty(res.ResourceId);
        var p = await ReadProjectFromRepo(res.TenantId, res.Id);
        Assert.False(p.Deleted);
    }
}