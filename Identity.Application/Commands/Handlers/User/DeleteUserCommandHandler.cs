using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user {UserId}", request.Id);
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.DeleteUser(request.PrincipalId);

        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} deleted", request.Id);
        return new DeleteUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}