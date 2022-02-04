using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public CreateUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = Domain.User.Initialize(request.PrincipalId, Guid.NewGuid(), request.FirstName, request.LastName,
            request.Email, request.Description ?? string.Empty, request.PictureUri ?? string.Empty);
        await _repository.AppendAsync(user, cancellationToken);
        return new CreateUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}