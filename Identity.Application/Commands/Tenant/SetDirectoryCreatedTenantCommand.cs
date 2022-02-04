using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class SetDirectoryCreatedTenantCommand : IRequest<SetDirectoryCreatedTenantCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid AdminGroupId { get; set; }
    public Guid UsersGroupId { get; set; }
}