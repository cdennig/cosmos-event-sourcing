using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateLanguageTenantCommandHandler : IRequestHandler<UpdateLanguageTenantCommand,
    UpdateLanguageTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly ILogger<UpdateLanguageTenantCommandHandler> _logger;

    public UpdateLanguageTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository,
        ILogger<UpdateLanguageTenantCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateLanguageTenantCommandResponse> Handle(
        UpdateLanguageTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating language for tenant {TenantId}", request.Id);
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateLanguage(request.PrincipalId, request.Language);
        await _repository.AppendAsync(tenant, cancellationToken);

        _logger.LogInformation("Language updated for tenant {TenantId}", request.Id);
        return new UpdateLanguageTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}