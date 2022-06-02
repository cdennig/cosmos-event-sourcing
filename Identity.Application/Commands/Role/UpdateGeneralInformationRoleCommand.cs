using Identity.Application.Commands.Responses.Role;
using MediatR;

namespace Identity.Application.Commands.Role;

public class UpdateGeneralInformationRoleCommand : IRequest<UpdateGeneralInformationRoleCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}