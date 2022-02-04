using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class CreateUserCommand : IRequest<CreateUserCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Description { get; set; }
    public string? PictureUri { get; set; }
}