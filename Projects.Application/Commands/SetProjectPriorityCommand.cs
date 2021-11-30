using System;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands
{
    public class SetProjectPriorityCommand : IRequest<SetProjectPriorityCommandResponse>
    {
        public Guid TenantId { get; set; }
        public Guid Id { get; set; }
        public ProjectPriority Priority { get; set; }
    }
}