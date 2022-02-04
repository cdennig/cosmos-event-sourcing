using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class CreateTenantCommand : IRequest<CreateTenantCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string? Location { get; set; }
    public string? PictureUri { get; set; }
}