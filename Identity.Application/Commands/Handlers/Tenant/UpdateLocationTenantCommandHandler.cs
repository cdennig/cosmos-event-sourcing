using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateLocationTenantCommandHandler : IRequestHandler<UpdateLocationTenantCommand,
    UpdateLocationTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly ILogger<UpdateLocationTenantCommandHandler> _logger;

    public UpdateLocationTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository,
        ILogger<UpdateLocationTenantCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateLocationTenantCommandResponse> Handle(
        UpdateLocationTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating location for tenant {TenantId}", request.Id);
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateLocation(request.PrincipalId, request.Location);
        await _repository.AppendAsync(tenant, cancellationToken);

        _logger.LogInformation("Location updated for tenant {TenantId}", request.Id);
        return new UpdateLocationTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}