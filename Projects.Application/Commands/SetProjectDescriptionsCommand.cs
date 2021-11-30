using System;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands
{
    public class SetProjectDescriptionsCommand : IRequest<SetProjectDescriptionsCommandResponse>
    {
        public Guid TenantId { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}