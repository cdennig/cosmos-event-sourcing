using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class SetPrimaryContactTenantCommandHandler : IRequestHandler<SetPrimaryContactTenantCommand,
    SetPrimaryContactTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;
    private readonly IUserService _userService;

    public SetPrimaryContactTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<SetPrimaryContactTenantCommandResponse> Handle(
        SetPrimaryContactTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        if (!await _userService.IsUserValidAsPrimaryContact(request.ContactId))
        {
            throw new Exception("User is not valid as primary contact");
        }
        var tenant = await _repository.RehydrateAsync(request.Id, cancellationToken);
        tenant.SetPrimaryContact(request.PrincipalId, request.ContactId);

        return new SetPrimaryContactTenantCommandResponse(tenant.Id, tenant.Version, tenant.ResourceId);
    }
}