using System;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands
{
    public class CreateProjectCommand : IRequest<CreateProjectCommandResponse>
    {
        public Guid TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public ProjectPriority Priority { get; set; }
    }
}