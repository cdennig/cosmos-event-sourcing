using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class UpdateEmailUserCommandHandler : IRequestHandler<UpdateEmailUserCommand,
    UpdateEmailUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<UpdateEmailUserCommandHandler> _logger;

    public UpdateEmailUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<UpdateEmailUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateEmailUserCommandResponse> Handle(UpdateEmailUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating email for user {UserId}", request.Id);
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UpdateEmail(request.PrincipalId, request.Email);

        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("Email for user {UserId} updated", request.Id);
        return new UpdateEmailUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}