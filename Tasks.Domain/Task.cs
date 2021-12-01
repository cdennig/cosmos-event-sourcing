using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using Tasks.Domain.Events;

namespace Tasks.Domain
{
    public class Task : BaseAggregateRoot<Guid, Task, Guid, Guid>
    {
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public DateTimeOffset? StartDate { get; private set; }
        public DateTimeOffset? EndDate { get; private set; }
        public TaskPriority? Priority { get; private set; }
        public bool Completed { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

        // ...in minutes
        public ulong TimeEstimation { get; private set; }

        private readonly List<TimeLogEntry> _timeLogEntries = new();

        public IReadOnlyCollection<TimeLogEntry> TimeLogEntries =>
            _timeLogEntries.ToImmutableArray();

        public Guid? ProjectId { get; private set; }

        // ResourceId depends on if task is assigned to a project or not
        public override string ResourceId =>
            (ProjectId == null || ProjectId == Guid.Empty)
                ? $"/orgs/{TenantId}/tasks/{Id}"
                : $"/orgs/{TenantId}/projects/{ProjectId}/tasks/{Id}";

        private Task(Guid tenantId, Guid taskId) : base(tenantId,
            taskId)
        {
        }

        private Task(Guid tenantId, Guid principalId, Guid taskId, string title, string description,
            Guid? projectId = null,
            DateTimeOffset? startDate = null,
            DateTimeOffset? endDate = null, TaskPriority priority = TaskPriority.Medium,
            ulong timeEstimation = 0) : base(tenantId,
            taskId)
        {
            var created = new TaskCreated(this, principalId, title, description, projectId, startDate, endDate,
                priority,
                timeEstimation);
            AddEvent(created);
        }

        public static Task Initialize(Guid tenantId, Guid principalId, Guid taskId, string title,
            string description = "",
            Guid? projectId = null,
            DateTimeOffset? startDate = null,
            DateTimeOffset? endDate = null, TaskPriority priority = TaskPriority.Medium, ulong timeEstimation = 0)
        {
            return new Task(tenantId, principalId, taskId, title, description, projectId, startDate, endDate, priority,
                timeEstimation);
        }

        private bool IsWritable()
        {
            return !Deleted && !Completed;
        }

        public void SetDescriptions(Guid by, string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var descriptionsUpdated = new TaskDescriptionsUpdated(this, by, title, description);
            AddEvent(descriptionsUpdated);
        }

        public void SetDates(Guid by, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("Task end date cannot be lower than start date.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var datesUpdated = new TaskDatesUpdated(this, by, startDate, endDate);
            AddEvent(datesUpdated);
        }

        public void SetPriority(Guid by, TaskPriority priority)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var priorityUpdated = new TaskPriorityUpdated(this, by, priority);
            AddEvent(priorityUpdated);
        }


        public void DeleteTask(Guid by)
        {
            if (Deleted)
                throw new ArgumentException("Task already deleted.");
            var deleted = new TaskDeleted(this, by);
            AddEvent(deleted);
        }

        public void UndeleteTask(Guid by)
        {
            if (!Deleted)
                throw new ArgumentException("Task not deleted.");
            var undeleted = new TaskUndeleted(this, by);
            AddEvent(undeleted);
        }

        public void SetComplete(Guid by)
        {
            if (!IsWritable())
                throw new ArgumentException("Task readonly.");
            var completed = new TaskSetComplete(this, by);
            AddEvent(completed);
        }

        public void SetIncomplete(Guid by)
        {
            if (!Completed)
                throw new ArgumentException("Task not completed.");
            var incomplete = new TaskSetIncomplete(this, by);
            AddEvent(incomplete);
        }

        public void SetTimeEstimation(Guid by, ulong estimation)
        {
            if (!IsWritable())
                throw new ArgumentException("Task readonly.");
            var estimated = new TaskEstimated(this, by, estimation);
            AddEvent(estimated);
        }

        public void AssignToProject(Guid by, Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Invalid Project Id.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var assigned = new TaskAssignedToProject(this, by, projectId);
            AddEvent(assigned);
        }


        public void RemoveFromProject(Guid by)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var removedFromProject = new TaskRemovedFromProject(this, by);
            AddEvent(removedFromProject);
        }

        public void LogTime(Guid by, ulong duration, string comment, DateOnly day)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var timeLogged = new TaskTimeLogged(this, by, day, comment, duration);
            AddEvent(timeLogged);
        }

        public void DeleteTimeLogEntry(Guid by, Guid entryId)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var timeLogEntryUpdated = new TaskTimeLogEntryDeleted(this, entryId, by);
            AddEvent(timeLogEntryUpdated);
        }

        public void ChangeTimeLogEntryComment(Guid by, Guid entryId, string comment)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var tle = TimeLogEntries.SingleOrDefault(t => t.Id == entryId);
            if (tle == null) throw new ArgumentException($"TimeLog entry not found: {entryId}");
            var tleCommentChanged = new TaskTimeLogEntryCommentChanged(this, tle, by, comment);
            AddEvent(tleCommentChanged);
        }

        protected override void Apply(IDomainEvent<Guid, Guid, Guid> @event)
        {
            ApplyEvent((dynamic) @event);
        }

        private void ApplyEvent(TaskCreated created)
        {
            Title = created.Title;
            Description = created.Description;
            ProjectId = created.ProjectId;
            StartDate = created.StartDate;
            EndDate = created.EndDate;
            Priority = created.Priority;
            Completed = false;
            TimeEstimation = created.TimeEstimation;
            CreatedAt = created.Timestamp;
            CreatedBy = created.RaisedBy;
        }

        private void ApplyEvent(TaskDescriptionsUpdated taskDescriptionsUpdated)
        {
            Title = taskDescriptionsUpdated.Title;
            Description = taskDescriptionsUpdated.Description;
            ModifiedAt = taskDescriptionsUpdated.Timestamp;
            ModifiedBy = taskDescriptionsUpdated.RaisedBy;
        }

        private void ApplyEvent(TaskDatesUpdated taskDatesUpdated)
        {
            StartDate = taskDatesUpdated.StartDate;
            EndDate = taskDatesUpdated.EndDate;
            ModifiedAt = taskDatesUpdated.Timestamp;
            ModifiedBy = taskDatesUpdated.RaisedBy;
        }

        private void ApplyEvent(TaskPriorityUpdated taskPriorityUpdated)
        {
            Priority = taskPriorityUpdated.NewPriority;
            ModifiedAt = taskPriorityUpdated.Timestamp;
            ModifiedBy = taskPriorityUpdated.RaisedBy;
        }

        private void ApplyEvent(TaskDeleted taskDeleted)
        {
            Deleted = true;
            DeletedAt = taskDeleted.Timestamp;
            DeletedBy = taskDeleted.RaisedBy;
        }

        private void ApplyEvent(TaskUndeleted taskUndeleted)
        {
            Deleted = false;
            ModifiedAt = taskUndeleted.Timestamp;
            ModifiedBy = taskUndeleted.RaisedBy;
        }

        private void ApplyEvent(TaskSetComplete complete)
        {
            Completed = true;
            CompletedAt = complete.Timestamp;
            ModifiedAt = complete.Timestamp;
            ModifiedBy = complete.RaisedBy;
        }

        private void ApplyEvent(TaskSetIncomplete incomplete)
        {
            Completed = false;
            ModifiedAt = incomplete.Timestamp;
            ModifiedBy = incomplete.RaisedBy;
            CompletedAt = null;
        }

        private void ApplyEvent(TaskAssignedToProject assignedToProject)
        {
            ProjectId = assignedToProject.NewProject;
            ModifiedAt = assignedToProject.Timestamp;
            ModifiedBy = assignedToProject.RaisedBy;
        }

        private void ApplyEvent(TaskRemovedFromProject removedFromProject)
        {
            ProjectId = null;
            ModifiedAt = removedFromProject.Timestamp;
            ModifiedBy = removedFromProject.RaisedBy;
        }

        private void ApplyEvent(TaskTimeLogged taskTimeLogged)
        {
            var tle = TimeLogEntry.Initialize(
                taskTimeLogged.TenantId,
                taskTimeLogged.RaisedBy,
                taskTimeLogged.TimeLogEntryId,
                this,
                taskTimeLogged.Day,
                taskTimeLogged.Comment,
                taskTimeLogged.Duration,
                taskTimeLogged.Timestamp
            );
            _timeLogEntries.Add(tle);
        }

        private void ApplyEvent(TaskEstimated taskEstimated)
        {
            TimeEstimation = taskEstimated.NewTimeEstimation;
            ModifiedAt = taskEstimated.Timestamp;
            ModifiedBy = taskEstimated.RaisedBy;
        }

        private void ApplyEvent(TaskTimeLogEntryDeleted entryDeleted)
        {
            var tle = _timeLogEntries.FirstOrDefault(e => e.Id ==
                                                          entryDeleted.TimeLogEntryId);
            if (tle != null) _timeLogEntries.Remove(tle);
        }

        private void ApplyEvent(TaskTimeLogEntryCommentChanged commentChanged)
        {
            var tle = _timeLogEntries.FirstOrDefault(e => e.Id ==
                                                          commentChanged.TimeLogEntryId);
            tle?.SetComment(commentChanged.RaisedBy, commentChanged.NewComment);
        }
    }
}