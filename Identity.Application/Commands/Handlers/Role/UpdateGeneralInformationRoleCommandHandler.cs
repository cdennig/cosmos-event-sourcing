using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using MediatR;

namespace Identity.Application.Commands.Handlers.Role;

public class UpdateGeneralInformationRoleCommandHandler : IRequestHandler<UpdateGeneralInformationRoleCommand,
    UpdateGeneralInformationRoleCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;

    public UpdateGeneralInformationRoleCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateGeneralInformationRoleCommandResponse> Handle(UpdateGeneralInformationRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description);
        await _repository.AppendAsync(role, cancellationToken);
        return new UpdateGeneralInformationRoleCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}