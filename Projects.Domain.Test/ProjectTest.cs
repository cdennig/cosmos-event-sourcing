using System;
using Projects.Shared.Aggregate;
using Xunit;

namespace Projects.Domain.Test
{
    public class ProjectTest
    {
        [Fact]
        public void Test1()
        {
            var p = Project.Initialize(Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            p.FinishProject();
        }
        
        [Fact]
        public void Test2()
        {
            var p = Project.Initialize(Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetDescriptions("New Title", "New description");
            p.SetDates(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            p.StartProject();
            p.PauseProject();
            p.ResumeProject();
            p.FinishProject();
            var pNew = BaseAggregateRoot<Project, Guid>.Create(p.DomainEvents);
        }
    }
}