using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class SetPrimaryContactTenantCommandHandler : IRequestHandler<SetPrimaryContactTenantCommand,
    SetPrimaryContactTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public SetPrimaryContactTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetPrimaryContactTenantCommandResponse> Handle(
        SetPrimaryContactTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.SetPrimaryContact(request.PrincipalId, request.ContactId);

        return new SetPrimaryContactTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}