using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Role;

public class RemoveRoleFromGroupCommandHandler : IRequestHandler<RemoveRoleFromGroupCommand,
    RemoveRoleFromGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly ILogger<AssignRoleToGroupCommandHandler> _logger;

    public RemoveRoleFromGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository,
        ILogger<AssignRoleToGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RemoveRoleFromGroupCommandResponse> Handle(RemoveRoleFromGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing role {RoleId} from group {GroupId} / tenant {TenantId}", request.Id,
            request.GroupId, request.TenantId);
        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.RemoveRoleFromGroup(request.PrincipalId, request.GroupId);
        await _repository.AppendAsync(role, cancellationToken);
        _logger.LogInformation("Removed role {RoleId} from group {GroupId} / tenant {TenantId}", request.Id,
            request.GroupId, request.TenantId);
        return new RemoveRoleFromGroupCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}