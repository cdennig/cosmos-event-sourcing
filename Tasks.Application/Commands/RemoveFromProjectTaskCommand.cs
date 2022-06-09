using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class RemoveFromProjectTaskCommand : IRequest<RemoveFromProjectTaskCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
}