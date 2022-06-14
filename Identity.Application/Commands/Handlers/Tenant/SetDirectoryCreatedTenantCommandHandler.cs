using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class SetDirectoryCreatedTenantCommandHandler : IRequestHandler<SetDirectoryCreatedTenantCommand,
    SetDirectoryCreatedTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly ILogger<SetDirectoryCreatedTenantCommandHandler> _logger;

    public SetDirectoryCreatedTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository,
        ILogger<SetDirectoryCreatedTenantCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetDirectoryCreatedTenantCommandResponse> Handle(
        SetDirectoryCreatedTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting DirectoryCreated property for tenant {TenantId}", request.Id);

        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.SetDirectoryCreated(request.PrincipalId, request.AdminGroupId, request.UsersGroupId, request.AdminRoleId,
            request.UsersRoleId);
        await _repository.AppendAsync(tenant, cancellationToken);

        _logger.LogInformation("DirectoryCreated property set for tenant {TenantId}", request.Id);
        return new SetDirectoryCreatedTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}