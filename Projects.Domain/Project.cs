using Projects.Domain.Events;
using ES.Shared.Aggregate;
using ES.Shared.Events;

namespace Projects.Domain;

public class Project : TenantAggregateRoot<Guid, Project, Guid, Guid>
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public DateTimeOffset? ActualStartDate { get; private set; }
    public DateTimeOffset? ActualEndDate { get; private set; }
    public ProjectStatus Status { get; private set; }
    public ProjectPriority Priority { get; private set; }

    public override string ResourceId => $"/t/{TenantId}/projects/{Id}";

    private Project(Guid tenantId, Guid projectId) : base(tenantId,
        projectId)
    {
    }

    private Project(Guid tenantId, Guid principalId, Guid projectId, string title, string description,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate, ProjectPriority priority = ProjectPriority.Medium) : base(tenantId,
        projectId)
    {
        var pc = new ProjectCreated(this, principalId, title, description, startDate, endDate, priority);
        AddEvent(pc);
    }

    public static Project Initialize(Guid tenantId, Guid principalId, Guid projectId, string title,
        string description = "",
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null, ProjectPriority priority = ProjectPriority.Medium)
    {
        return new Project(tenantId, principalId, projectId, title, description, startDate, endDate, priority);
    }

    private bool IsWritable()
    {
        if (Deleted)
            return false;

        switch (Status)
        {
            case ProjectStatus.Cancelled:
            case ProjectStatus.Finished:
                return false;
            case ProjectStatus.New:
            case ProjectStatus.Started:
            case ProjectStatus.Paused:
            case ProjectStatus.Resumed:
                return true;
            default:
                return false;
        }
    }

    public void StartProject(Guid by)
    {
        if (Status != ProjectStatus.New)
            throw new ArgumentException("Project cannot be started in current state.");
        var ps = new ProjectStarted(this, by, DateTimeOffset.UtcNow);
        AddEvent(ps);
    }

    public void PauseProject(Guid by)
    {
        if (Status is not ProjectStatus.Started and not ProjectStatus.Resumed)
            throw new ArgumentException("Project cannot be paused in current state.");
        var pp = new ProjectPaused(this, by);
        AddEvent(pp);
    }

    public void CancelProject(Guid by)
    {
        if (Status is ProjectStatus.Cancelled or ProjectStatus.Finished)
            throw new ArgumentException("Project cannot be cancelled in current state.");
        var pc = new ProjectCancelled(this, by);
        AddEvent(pc);
    }

    public void ResumeProject(Guid by)
    {
        if (Status != ProjectStatus.Paused)
            throw new ArgumentException("Project cannot be resumed in current state.");
        var pr = new ProjectResumed(this, by);
        AddEvent(pr);
    }

    public void FinishProject(Guid by)
    {
        if (Status is ProjectStatus.Finished or ProjectStatus.Cancelled or ProjectStatus.New)
            throw new ArgumentException("Project cannot be finished in current state.");
        var pf = new ProjectFinished(this, by, DateTimeOffset.UtcNow);
        AddEvent(pf);
    }

    public void SetDescriptions(Guid by, string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty.");
        if (!IsWritable())
            throw new Exception("Project readonly");
        var pdes = new ProjectDescriptionsUpdated(this, by, title, description);
        AddEvent(pdes);
    }

    public void SetDates(Guid by, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("Project end date cannot be lower than start date.");
        if (!IsWritable())
            throw new Exception("Project readonly");
        var pdates = new ProjectDatesUpdated(this, by, startDate, endDate);
        AddEvent(pdates);
    }

    public void SetPriority(Guid by, ProjectPriority priority)
    {
        if (!IsWritable())
            throw new Exception("Project readonly");
        var pprio = new ProjectPriorityUpdated(this, by, priority);
        AddEvent(pprio);
    }

    public void DeleteProject(Guid by)
    {
        if (Deleted)
            throw new ArgumentException("Project already deleted.");
        var deleted = new ProjectDeleted(this, by);
        AddEvent(deleted);
    }

    public void UndeleteProject(Guid by)
    {
        if (!Deleted)
            throw new ArgumentException("Project not deleted.");
        var undeleted = new ProjectUndeleted(this, by);
        AddEvent(undeleted);
    }

    protected override void Apply(ITenantDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic) @event);
    }

    private void ApplyEvent(ProjectCreated projectCreated)
    {
        Title = projectCreated.Title;
        Description = projectCreated.Description;
        StartDate = projectCreated.StartDate;
        EndDate = projectCreated.EndDate;
        Status = ProjectStatus.New;
        Priority = projectCreated.Priority;
        CreatedAt = projectCreated.Timestamp;
        CreatedBy = projectCreated.RaisedBy;
    }

    private void ApplyEvent(ProjectStarted projectStarted)
    {
        Status = ProjectStatus.Started;
        StartDate ??= projectStarted.ActualStartDate;
        ActualStartDate = projectStarted.ActualStartDate;
        ModifiedAt = projectStarted.Timestamp;
        ModifiedBy = projectStarted.RaisedBy;
    }

    private void ApplyEvent(ProjectPaused projectPaused)
    {
        Status = ProjectStatus.Paused;
        ModifiedAt = projectPaused.Timestamp;
        ModifiedBy = projectPaused.RaisedBy;
    }

    private void ApplyEvent(ProjectCancelled projectCancelled)
    {
        Status = ProjectStatus.Cancelled;
        ModifiedAt = projectCancelled.Timestamp;
        ModifiedBy = projectCancelled.RaisedBy;
    }

    private void ApplyEvent(ProjectResumed projectResumed)
    {
        Status = ProjectStatus.Resumed;
        ModifiedAt = projectResumed.Timestamp;
        ModifiedBy = projectResumed.RaisedBy;
    }

    private void ApplyEvent(ProjectFinished projectFinished)
    {
        Status = ProjectStatus.Finished;
        ActualEndDate = projectFinished.ActualEndDate;
        EndDate ??= projectFinished.ActualEndDate;
        ModifiedAt = projectFinished.Timestamp;
        ModifiedBy = projectFinished.RaisedBy;
    }

    private void ApplyEvent(ProjectDescriptionsUpdated projectDescriptionsUpdated)
    {
        Title = projectDescriptionsUpdated.Title;
        Description = projectDescriptionsUpdated.Description;
        ModifiedAt = projectDescriptionsUpdated.Timestamp;
        ModifiedBy = projectDescriptionsUpdated.RaisedBy;
    }

    private void ApplyEvent(ProjectDatesUpdated projectDatesUpdated)
    {
        StartDate = projectDatesUpdated.StartDate;
        EndDate = projectDatesUpdated.EndDate;
        ModifiedAt = projectDatesUpdated.Timestamp;
        ModifiedBy = projectDatesUpdated.RaisedBy;
    }

    private void ApplyEvent(ProjectPriorityUpdated projectPriorityUpdated)
    {
        Priority = projectPriorityUpdated.NewPriority;
        ModifiedAt = projectPriorityUpdated.Timestamp;
        ModifiedBy = projectPriorityUpdated.RaisedBy;
    }

    private void ApplyEvent(ProjectDeleted projectDeleted)
    {
        Deleted = true;
        DeletedAt = projectDeleted.Timestamp;
        DeletedBy = projectDeleted.RaisedBy;
    }

    private void ApplyEvent(ProjectUndeleted projectUndeleted)
    {
        Deleted = false;
        ModifiedAt = projectUndeleted.Timestamp;
        ModifiedBy = projectUndeleted.RaisedBy;
    }
}