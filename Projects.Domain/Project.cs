using System;
using Projects.Domain.Events;
using Projects.Shared.Aggregate;
using Projects.Shared.Events;

namespace Projects.Domain
{
    public class Project : BaseAggregateRoot<Project, Guid>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset? StartDate { get; private set; }
        public DateTimeOffset? EndDate { get; private set; }
        public DateTimeOffset? ActualStartDate { get; private set; }
        public DateTimeOffset? ActualEndDate { get; private set; }
        public ProjectStatus Status { get; private set; }
        public ProjectPriority Priority { get; private set; }

        private Project()
        {
        }

        private Project(Guid projectId, string title, DateTimeOffset startDate) : base(projectId)
        {
            var pc = new ProjectCreated(this, title, startDate);
            AddEvent(pc);
        }

        public static Project Initialize(Guid projectId, string title, DateTimeOffset startDate)
        {
            return new Project(projectId, title, startDate);
        }

        private bool IsWritable()
        {
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

        protected override void Apply(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case ProjectCreated projectCreated:
                    Id = projectCreated.AggregateId;
                    Title = projectCreated.Title;
                    StartDate = projectCreated.StartDate;
                    Status = ProjectStatus.New;
                    Priority = ProjectPriority.Medium;
                    CreatedAt = projectCreated.Timestamp;
                    break;
                case ProjectStarted projectStarted:
                    Status = ProjectStatus.Started;
                    ActualStartDate = projectStarted.ActualStartDate;
                    ModifiedAt = projectStarted.Timestamp;
                    break;
                case ProjectPaused projectPaused:
                    Status = ProjectStatus.Paused;
                    ModifiedAt = projectPaused.Timestamp;
                    break;
                case ProjectCancelled projectCancelled:
                    Status = ProjectStatus.Cancelled;
                    ModifiedAt = projectCancelled.Timestamp;
                    break;
                case ProjectResumed projectResumed:
                    Status = ProjectStatus.Resumed;
                    ModifiedAt = projectResumed.Timestamp;
                    break;
                case ProjectFinished projectFinished:
                    Status = ProjectStatus.Finished;
                    ActualEndDate = projectFinished.ActualEndDate;
                    EndDate ??= projectFinished.ActualEndDate;
                    ModifiedAt = projectFinished.Timestamp;
                    break;
                case ProjectDescriptionsUpdated projectDescriptionsUpdated:
                    Title = projectDescriptionsUpdated.Title;
                    Description = projectDescriptionsUpdated.Description;
                    ModifiedAt = projectDescriptionsUpdated.Timestamp;
                    break;
                case ProjectDatesUpdated projectDatesUpdated:
                    StartDate = projectDatesUpdated.StartDate;
                    EndDate = projectDatesUpdated.EndDate;
                    ModifiedAt = projectDatesUpdated.Timestamp;
                    break;
                case ProjectPriorityUpdated projectPriorityUpdated:
                    Priority = projectPriorityUpdated.NewPriority;
                    ModifiedAt = projectPriorityUpdated.Timestamp;
                    break;
            }
        }
    }
}