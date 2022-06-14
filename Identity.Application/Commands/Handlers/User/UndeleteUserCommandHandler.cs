using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class UndeleteUserCommandHandler : IRequestHandler<UndeleteUserCommand, UndeleteUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<UndeleteUserCommandHandler> _logger;

    public UndeleteUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<UndeleteUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UndeleteUserCommandResponse> Handle(UndeleteUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Undeleting user {UserId}", request.Id);
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UndeleteUser(request.PrincipalId);

        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} undeleted", request.Id);
        return new UndeleteUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}