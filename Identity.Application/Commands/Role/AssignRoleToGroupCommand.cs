using Identity.Application.Commands.Responses.Role;
using MediatR;

namespace Identity.Application.Commands.Role;

public class AssignRoleToGroupCommand : IRequest<AssignRoleToGroupCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
}