using System;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands
{
    public class SetProjectDatesCommand : IRequest<SetProjectDatesCommandResponse>
    {
        public Guid TenantId { get; set; }
        public Guid PrincipalId { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}