using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class UpdateLanguageTenantCommandHandler : IRequestHandler<UpdateLanguageTenantCommand,
    UpdateLanguageTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public UpdateLanguageTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UpdateLanguageTenantCommandResponse> Handle(
        UpdateLanguageTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.UpdateLanguage(request.PrincipalId, request.Language);
        await _repository.AppendAsync(tenant, cancellationToken);
        
        return new UpdateLanguageTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}