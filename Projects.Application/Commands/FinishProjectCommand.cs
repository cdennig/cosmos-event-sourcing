using Projects.Application.Commands.Responses;
using MediatR;

namespace Projects.Application.Commands;

public class FinishProjectCommand : IRequest<FinishProjectCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
}