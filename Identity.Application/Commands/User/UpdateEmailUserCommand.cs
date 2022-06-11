using ES.Shared.Cache;
using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class UpdateEmailUserCommand : IRequest<UpdateEmailUserCommandResponse>, IInvalidatesCacheCommand
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string CacheKey => $"user/{Id}";
}