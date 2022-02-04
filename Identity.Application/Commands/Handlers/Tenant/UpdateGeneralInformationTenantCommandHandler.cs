using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateGeneralInformationTenantCommandHandler : IRequestHandler<UpdateGeneralInformationTenantCommand,
    UpdateGeneralInformationTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public UpdateGeneralInformationTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateGeneralInformationTenantCommandResponse> Handle(
        UpdateGeneralInformationTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description, request.PictureUri);

        return new UpdateGeneralInformationTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}