using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user with properties: {@CreateUserCommand}", request);
        var user = Domain.User.Initialize(request.PrincipalId, Guid.NewGuid(), request.FirstName, request.LastName,
            request.Email, request.Description ?? string.Empty, request.PictureUri ?? string.Empty);
        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} created", user.Id);
        return new CreateUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}