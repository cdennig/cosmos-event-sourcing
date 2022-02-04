using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public DeleteUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.DeleteUser(request.PrincipalId);
        
        return new DeleteUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}