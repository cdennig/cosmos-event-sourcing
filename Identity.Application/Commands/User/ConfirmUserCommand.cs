using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class ConfirmUserCommand : IRequest<ConfirmUserCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
}