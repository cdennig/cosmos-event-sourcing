using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class SetDirectoryCreatedTenantCommandHandler : IRequestHandler<SetDirectoryCreatedTenantCommand,
    SetDirectoryCreatedTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public SetDirectoryCreatedTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetDirectoryCreatedTenantCommandResponse> Handle(
        SetDirectoryCreatedTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.SetDirectoryCreated(request.PrincipalId, request.AdminGroupId, request.UsersGroupId, request.AdminRoleId,
            request.UsersRoleId);
        await _repository.AppendAsync(tenant, cancellationToken);

        return new SetDirectoryCreatedTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}