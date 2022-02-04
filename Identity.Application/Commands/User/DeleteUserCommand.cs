using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class DeleteUserCommand : IRequest<DeleteUserCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
}