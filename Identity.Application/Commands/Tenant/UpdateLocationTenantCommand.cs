using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class UpdateLocationTenantCommand : IRequest<UpdateLocationTenantCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Location { get; set; }
}