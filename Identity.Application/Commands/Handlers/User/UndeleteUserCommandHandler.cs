using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class UndeleteUserCommandHandler : IRequestHandler<UndeleteUserCommand, UndeleteUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public UndeleteUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UndeleteUserCommandResponse> Handle(UndeleteUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UndeleteUser(request.PrincipalId);
        
        await _repository.AppendAsync(user, cancellationToken);
        
        return new UndeleteUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}