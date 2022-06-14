using ES.Shared.Repository;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.User;

public class UpdatePersonalInformationUserCommandHandler : IRequestHandler<UpdatePersonalInformationUserCommand,
    UpdatePersonalInformationUserCommandResponse>
{
    private readonly IEventsRepository<Domain.User, Guid, Guid> _repository;
    private readonly ILogger<UpdatePersonalInformationUserCommandHandler> _logger;

    public UpdatePersonalInformationUserCommandHandler(IEventsRepository<Domain.User, Guid, Guid> repository,
        ILogger<UpdatePersonalInformationUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdatePersonalInformationUserCommandResponse> Handle(UpdatePersonalInformationUserCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating personal information for user {UserId}", request.Id);
        var user = await _repository.RehydrateAsync(request.Id, cancellationToken);
        user.UpdatePersonalInformation(request.PrincipalId, request.FirstName, request.LastName, request.Description,
            request.PictureUri);

        await _repository.AppendAsync(user, cancellationToken);

        _logger.LogInformation("Personal information for user {UserId} updated", request.Id);
        return new UpdatePersonalInformationUserCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}