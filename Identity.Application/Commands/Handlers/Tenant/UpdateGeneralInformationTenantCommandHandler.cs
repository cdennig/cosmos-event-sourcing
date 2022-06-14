using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateGeneralInformationTenantCommandHandler : IRequestHandler<UpdateGeneralInformationTenantCommand,
    UpdateGeneralInformationTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly ILogger<UpdateGeneralInformationTenantCommandHandler> _logger;

    public UpdateGeneralInformationTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository,
        ILogger<UpdateGeneralInformationTenantCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateGeneralInformationTenantCommandResponse> Handle(
        UpdateGeneralInformationTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating general information for tenant {TenantId}", request.Id);
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description, request.PictureUri);
        await _repository.AppendAsync(tenant, cancellationToken);

        _logger.LogInformation("General information updated for tenant {TenantId}", request.Id);
        return new UpdateGeneralInformationTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}