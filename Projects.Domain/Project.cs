using System;
using Projects.Domain.Events;
using ES.Shared.Aggregate;
using ES.Shared.Events;

namespace Projects.Domain
{
    public class Project : BaseAggregateRoot<Guid, Project, Guid>
    {
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public DateTimeOffset? StartDate { get; private set; }
        public DateTimeOffset? EndDate { get; private set; }
        public DateTimeOffset? ActualStartDate { get; private set; }
        public DateTimeOffset? ActualEndDate { get; private set; }
        public ProjectStatus Status { get; private set; }
        public ProjectPriority Priority { get; private set; }

        public override string ResourceId => $"/org/{TenantId}/project/{Id}";

        private Project(Guid tenantId, Guid projectId) : base(tenantId,
            projectId)
        {
        }

        private Project(Guid tenantId, Guid projectId, string title, DateTimeOffset startDate) : base(tenantId,
            projectId)
        {
            var pc = new ProjectCreated(this, title, startDate);
            AddEvent(pc);
        }

        public static Project Initialize(Guid tenantId, Guid projectId, string title, DateTimeOffset startDate)
        {
            return new Project(tenantId, projectId, title, startDate);
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

        public void StartProject()
        {
            if (Status != ProjectStatus.New)
                throw new ArgumentException("Project cannot be started in current state.");
            var ps = new ProjectStarted(this, DateTimeOffset.UtcNow);
            AddEvent(ps);
        }

        public void PauseProject()
        {
            if (Status is not ProjectStatus.Started and not ProjectStatus.Resumed)
                throw new ArgumentException("Project cannot be paused in current state.");
            var pp = new ProjectPaused(this);
            AddEvent(pp);
        }

        public void CancelProject()
        {
            if (Status is ProjectStatus.Cancelled or ProjectStatus.Finished)
                throw new ArgumentException("Project cannot be cancelled in current state.");
            var pc = new ProjectCancelled(this);
            AddEvent(pc);
        }

        public void ResumeProject()
        {
            if (Status != ProjectStatus.Paused)
                throw new ArgumentException("Project cannot be resumed in current state.");
            var pr = new ProjectResumed(this);
            AddEvent(pr);
        }

        public void FinishProject()
        {
            if (Status is ProjectStatus.Finished or ProjectStatus.Cancelled or ProjectStatus.New)
                throw new ArgumentException("Project cannot be finished in current state.");
            var pf = new ProjectFinished(this, DateTimeOffset.UtcNow);
            AddEvent(pf);
        }

        public void SetDescriptions(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Project title cannot be empty.");
            if (!IsWritable())
                throw new Exception("Project readonly");
            var pdes = new ProjectDescriptionsUpdated(this, title, description);
            AddEvent(pdes);
        }

        public void SetDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("Project end date cannot be lower than start date.");
            if (!IsWritable())
                throw new Exception("Project readonly");
            var pdates = new ProjectDatesUpdated(this, startDate, endDate);
            AddEvent(pdates);
        }

        public void SetPriority(ProjectPriority priority)
        {
            if (!IsWritable())
                throw new Exception("Project readonly");
            var pprio = new ProjectPriorityUpdated(this, priority);
            AddEvent(pprio);
        }

        public void DeleteProject()
        {
            var pdeleted = new ProjectDeleted(this);
            AddEvent(pdeleted);
        }

        public void UndeleteProject()
        {
            var pundeleted = new ProjectUndeleted(this);
            AddEvent(pundeleted);
        }

        protected override void Apply(IDomainEvent<Guid, Guid> @event)
        {
            ApplyEvent((dynamic)@event);
        }

        private void ApplyEvent(ProjectCreated projectCreated)
        {
            Title = projectCreated.Title;
            StartDate = projectCreated.StartDate;
            Status = ProjectStatus.New;
            Priority = ProjectPriority.Medium;
            CreatedAt = projectCreated.Timestamp;
        }

        private void ApplyEvent(ProjectStarted projectStarted)
        {
            Status = ProjectStatus.Started;
            ActualStartDate = projectStarted.ActualStartDate;
            ModifiedAt = projectStarted.Timestamp;
        }

        private void ApplyEvent(ProjectPaused projectPaused)
        {
            Status = ProjectStatus.Paused;
            ModifiedAt = projectPaused.Timestamp;
        }

        private void ApplyEvent(ProjectCancelled projectCancelled)
        {
            Status = ProjectStatus.Cancelled;
            ModifiedAt = projectCancelled.Timestamp;
        }

        private void ApplyEvent(ProjectResumed projectResumed)
        {
            Status = ProjectStatus.Resumed;
            ModifiedAt = projectResumed.Timestamp;
        }

        private void ApplyEvent(ProjectFinished projectFinished)
        {
            Status = ProjectStatus.Finished;
            ActualEndDate = projectFinished.ActualEndDate;
            EndDate ??= projectFinished.ActualEndDate;
            ModifiedAt = projectFinished.Timestamp;
        }

        private void ApplyEvent(ProjectDescriptionsUpdated projectDescriptionsUpdated)
        {
            Title = projectDescriptionsUpdated.Title;
            Description = projectDescriptionsUpdated.Description;
            ModifiedAt = projectDescriptionsUpdated.Timestamp;
        }

        private void ApplyEvent(ProjectDatesUpdated projectDatesUpdated)
        {
            StartDate = projectDatesUpdated.StartDate;
            EndDate = projectDatesUpdated.EndDate;
            ModifiedAt = projectDatesUpdated.Timestamp;
        }

        private void ApplyEvent(ProjectPriorityUpdated projectPriorityUpdated)
        {
            Priority = projectPriorityUpdated.NewPriority;
            ModifiedAt = projectPriorityUpdated.Timestamp;
        }

        private void ApplyEvent(ProjectDeleted projectDeleted)
        {
            Deleted = true;
            DeletedAt = projectDeleted.Timestamp;
        }
        private void ApplyEvent(ProjectUndeleted projectUndeleted)
        {
            Deleted = false;
            ModifiedAt = projectUndeleted.Timestamp;
        }
    }
}