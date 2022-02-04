using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class UpdateEmailUserCommandHandler : IRequestHandler<UpdateEmailUserCommand,
    UpdateEmailUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public UpdateEmailUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateEmailUserCommandResponse> Handle(UpdateEmailUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UpdateEmail(request.PrincipalId, request.Email);

        return new UpdateEmailUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}