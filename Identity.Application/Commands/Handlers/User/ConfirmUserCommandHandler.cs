using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class ConfirmUserCommandHandler : IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<ConfirmUserCommandHandler> _logger;

    public ConfirmUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<ConfirmUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ConfirmUserCommandResponse> Handle(ConfirmUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Confirming user {UserId}", request.Id);
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.ConfirmUser(request.PrincipalId);

        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} confirmed", request.Id);
        return new ConfirmUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}