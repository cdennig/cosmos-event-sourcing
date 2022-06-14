using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly ILogger<CreateTenantCommandHandler> _logger;
    public CreateTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository, ILogger<CreateTenantCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateTenantCommandResponse> Handle(CreateTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating tenant with properties: {@CreateTenantCommand}", request);
        var tenant = Domain.Tenant.Initialize(request.PrincipalId, Guid.NewGuid(), request.Name, request.Description,
            request.Language, request.Location ?? string.Empty, request.PictureUri ?? string.Empty);
        await _repository.AppendAsync(tenant, cancellationToken);
        _logger.LogInformation("Tenant created: {TenantId}", tenant.Id);
        return new CreateTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}