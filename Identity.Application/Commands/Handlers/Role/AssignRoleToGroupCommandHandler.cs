using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using Identity.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Role;

public class AssignRoleToGroupCommandHandler : IRequestHandler<AssignRoleToGroupCommand,
    AssignRoleToGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly IGroupService _groupService;
    private readonly ILogger<AssignRoleToGroupCommandHandler> _logger;

    public AssignRoleToGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository, IGroupService groupService,
        ILogger<AssignRoleToGroupCommandHandler> logger)
    {
        _repository = repository;
        _groupService = groupService;
        _logger = logger;
    }

    public async Task<AssignRoleToGroupCommandResponse> Handle(AssignRoleToGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning role {RoleId} to group {GroupId} / tenant {TenantId}", request.Id,
            request.GroupId, request.TenantId);
        if (!await _groupService.IsGroupValidForRoleAssignment(request.TenantId, request.GroupId))
        {
            _logger.LogWarning("Group {GroupId} not valid for role assignment", request.GroupId);
            throw new Exception("Group is not valid for role assignment");
        }

        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.AssignRoleToGroup(request.PrincipalId, request.GroupId);
        await _repository.AppendAsync(role, cancellationToken);
        _logger.LogInformation("Assigned role {RoleId} to group {GroupId} / tenant {TenantId}", request.Id,
            request.GroupId, request.TenantId);
        return new AssignRoleToGroupCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}