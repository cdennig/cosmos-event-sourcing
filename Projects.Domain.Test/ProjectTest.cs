using System;
using Projects.Shared.Aggregate;
using Xunit;

namespace Projects.Domain.Test
{
    public class ProjectTest
    {
        [Fact]
        public void TestWorkflow()
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
        public void TestPriority()
        {
            var p = Project.Initialize(Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.SetPriority(ProjectPriority.VeryHigh);
            Assert.Equal(ProjectPriority.VeryHigh, p.Priority);
        }
        
        [Fact]
        public void TestProjectReadOnly()
        {
            var p = Project.Initialize(Guid.NewGuid(), "Test Project", DateTimeOffset.UtcNow);
            p.StartProject();
            p.FinishProject();
            void Action() => p.SetDescriptions("Should throw", "an exception");
            Assert.Throws<Exception>(Action);
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