using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class ConfirmUserCommandHandler : IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public ConfirmUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<ConfirmUserCommandResponse> Handle(ConfirmUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.ConfirmUser(request.PrincipalId);
        
        return new ConfirmUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}