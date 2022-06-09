using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class AssignToProjectTaskCommand : IRequest<AssignToProjectTaskCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
}