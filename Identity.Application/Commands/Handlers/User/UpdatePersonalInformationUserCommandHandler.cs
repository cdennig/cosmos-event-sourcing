using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;

namespace Identity.Application.Commands.Handlers.User;

public class UpdatePersonalInformationUserCommandHandler : IRequestHandler<UpdatePersonalInformationUserCommand,
    UpdatePersonalInformationUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;

    public UpdatePersonalInformationUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdatePersonalInformationUserCommandResponse> Handle(UpdatePersonalInformationUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UpdatePersonalInformation(request.PrincipalId, request.FirstName, request.LastName, request.Description,
            request.PictureUri);

        return new UpdatePersonalInformationUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}