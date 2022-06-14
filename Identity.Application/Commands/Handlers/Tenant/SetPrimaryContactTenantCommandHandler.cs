using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using Identity.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Tenant;

public class SetPrimaryContactTenantCommandHandler : IRequestHandler<SetPrimaryContactTenantCommand,
    SetPrimaryContactTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly IUserService _userService;
    private readonly ILogger<SetPrimaryContactTenantCommandHandler> _logger;

    public SetPrimaryContactTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository,
        IUserService userService, ILogger<SetPrimaryContactTenantCommandHandler> logger)
    {
        _repository = repository;
        _userService = userService;
        _logger = logger;
    }

    public async Task<SetPrimaryContactTenantCommandResponse> Handle(
        SetPrimaryContactTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting primary contact {PrimaryContactId} for tenant {TenantId}", request.PrincipalId,
            request.Id);
        if (!await _userService.IsUserValidAsPrimaryContact(request.ContactId))
        {
            _logger.LogWarning("Contact {PrimaryContactId} is not valid as primary contact for tenant {TenantId}",
                request.PrincipalId, request.Id);
            throw new Exception("User is not valid as primary contact");
        }

        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.SetPrimaryContact(request.PrincipalId, request.ContactId);
        await _repository.AppendAsync(tenant, cancellationToken);

        _logger.LogInformation("Primary contact {PrimaryContactId} set for tenant {TenantId}", request.PrincipalId,
            request.Id);
        return new SetPrimaryContactTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}