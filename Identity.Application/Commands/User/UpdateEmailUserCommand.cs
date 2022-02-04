using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class UpdateEmailUserCommand : IRequest<UpdateEmailUserCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Email { get; set; }
}