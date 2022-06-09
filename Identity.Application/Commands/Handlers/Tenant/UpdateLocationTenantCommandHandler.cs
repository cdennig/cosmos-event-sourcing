using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateLocationTenantCommandHandler : IRequestHandler<UpdateLocationTenantCommand,
    UpdateLocationTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public UpdateLocationTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateLocationTenantCommandResponse> Handle(
        UpdateLocationTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateLocation(request.PrincipalId, request.Location);
        await _repository.AppendAsync(tenant, cancellationToken);
        
        return new UpdateLocationTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}