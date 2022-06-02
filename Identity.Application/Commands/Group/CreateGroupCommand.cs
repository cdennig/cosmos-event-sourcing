using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Group;

public class CreateGroupCommand : IRequest<CreateGroupCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? PictureUri { get; set; }
}