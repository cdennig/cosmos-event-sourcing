using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class UpdateGeneralInformationGroupCommandHandler : IRequestHandler<UpdateGeneralInformationGroupCommand,
    UpdateGeneralInformationGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;

    public UpdateGeneralInformationGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateGeneralInformationGroupCommandResponse> Handle(UpdateGeneralInformationGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description, request.PictureUri);
        await _repository.AppendAsync(group, cancellationToken);
        return new UpdateGeneralInformationGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}