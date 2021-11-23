using System;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using Tasks.Domain.Events;

namespace Tasks.Domain
{
    public class Task : BaseAggregateRoot<Guid, Task, Guid>
    {
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public DateTimeOffset? StartDate { get; private set; }
        public DateTimeOffset? EndDate { get; private set; }
        public TaskPriority Priority { get; private set; }
        public bool Completed { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

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

        private Task(Guid tenantId, Guid taskId, string title, string description, DateTimeOffset? startDate,
            DateTimeOffset? endDate, TaskPriority priority = TaskPriority.Medium) : base(tenantId,
            taskId)
        {
            var created = new TaskCreated(this, title, description, startDate, endDate, priority);
            AddEvent(created);
        }

        public static Task Initialize(Guid tenantId, Guid taskId, string title, string description = "",
            DateTimeOffset? startDate = null,
            DateTimeOffset? endDate = null, TaskPriority priority = TaskPriority.Medium)
        {
            return new Task(tenantId, taskId, title, description, startDate, endDate, priority);
        }

        private bool IsWritable()
        {
            return !Deleted && !Completed;
        }

        public void SetDescriptions(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Task title cannot be empty.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var descriptionsUpdated = new TaskDescriptionsUpdated(this, title, description);
            AddEvent(descriptionsUpdated);
        }

        public void SetDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("Task end date cannot be lower than start date.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var datesUpdated = new TaskDatesUpdated(this, startDate, endDate);
            AddEvent(datesUpdated);
        }

        public void SetPriority(TaskPriority priority)
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var priorityUpdated = new TaskPriorityUpdated(this, priority);
            AddEvent(priorityUpdated);
        }


        public void DeleteTask()
        {
            if (Deleted)
                throw new ArgumentException("Task already deleted.");
            var deleted = new TaskDeleted(this);
            AddEvent(deleted);
        }

        public void UndeleteTask()
        {
            if (!Deleted)
                throw new ArgumentException("Task not deleted.");
            var undeleted = new TaskUndeleted(this);
            AddEvent(undeleted);
        }

        public void SetComplete()
        {
            if (!IsWritable())
                throw new ArgumentException("Task readonly.");
            var completed = new TaskSetComplete(this);
            AddEvent(completed);
        }

        public void SetIncomplete()
        {
            if (!Completed)
                throw new ArgumentException("Task not completed.");
            var incomplete = new TaskSetIncomplete(this);
            AddEvent(incomplete);
        }

        public void AssignToProject(Guid projectId)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("Invalid Project Id.");
            if (!IsWritable())
                throw new Exception("Task readonly");
            var assigned = new TaskAssignedToProject(this, projectId);
            AddEvent(assigned);
        }
        
        
        public void RemoveFromProject()
        {
            if (!IsWritable())
                throw new Exception("Task readonly");
            var removedFromProject = new TaskRemovedFromProject(this);
            AddEvent(removedFromProject);
        }

        protected override void Apply(IDomainEvent<Guid, Guid> @event)
        {
            ApplyEvent((dynamic)@event);
        }

        private void ApplyEvent(TaskCreated created)
        {
            Title = created.Title;
            Description = created.Description;
            StartDate = created.StartDate;
            EndDate = created.EndDate;
            Priority = created.Priority;
            Completed = false;
            CreatedAt = created.Timestamp;
        }

        private void ApplyEvent(TaskDescriptionsUpdated taskDescriptionsUpdated)
        {
            Title = taskDescriptionsUpdated.Title;
            Description = taskDescriptionsUpdated.Description;
            ModifiedAt = taskDescriptionsUpdated.Timestamp;
        }

        private void ApplyEvent(TaskDatesUpdated taskDatesUpdated)
        {
            StartDate = taskDatesUpdated.StartDate;
            EndDate = taskDatesUpdated.EndDate;
            ModifiedAt = taskDatesUpdated.Timestamp;
        }

        private void ApplyEvent(TaskPriorityUpdated taskPriorityUpdated)
        {
            Priority = taskPriorityUpdated.NewPriority;
            ModifiedAt = taskPriorityUpdated.Timestamp;
        }
        
        private void ApplyEvent(TaskDeleted taskDeleted)
        {
            Deleted = true;
            DeletedAt = taskDeleted.Timestamp;
        }

        private void ApplyEvent(TaskUndeleted taskUndeleted)
        {
            Deleted = false;
            ModifiedAt = taskUndeleted.Timestamp;
        }
        
        private void ApplyEvent(TaskSetComplete complete)
        {
            Completed = true;
            CompletedAt = complete.Timestamp;
            ModifiedAt = complete.Timestamp;
        }

        private void ApplyEvent(TaskSetIncomplete incomplete)
        {
            Completed = false;
            ModifiedAt = incomplete.Timestamp;
            CompletedAt = null;
        }

        private void ApplyEvent(TaskAssignedToProject assignedToProject)
        {
            ProjectId = assignedToProject.NewProject;
            ModifiedAt = assignedToProject.Timestamp;
        }
        
        private void ApplyEvent(TaskRemovedFromProject removedFromProject)
        {
            ProjectId = null;
            ModifiedAt = removedFromProject.Timestamp;
        }
    }
}