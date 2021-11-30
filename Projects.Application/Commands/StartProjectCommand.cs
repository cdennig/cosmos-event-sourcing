using System;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands
{
    public class StartProjectCommand : IRequest<StartProjectCommandResponse>
    {
        public Guid TenantId { get; set; }
        public Guid Id { get; set; }
    }
}