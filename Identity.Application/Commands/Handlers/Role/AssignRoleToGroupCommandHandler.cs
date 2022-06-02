using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Role;

public class AssignRoleToGroupCommandHandler : IRequestHandler<AssignRoleToGroupCommand,
    AssignRoleToGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly IGroupService _groupService;

    public AssignRoleToGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository, IGroupService groupService)
    {
        _repository = repository;
        _groupService = groupService;
    }

    public async Task<AssignRoleToGroupCommandResponse> Handle(AssignRoleToGroupCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _groupService.IsGroupValidForRoleAssignment(request.TenantId, request.GroupId))
        {
            throw new Exception("Group is not valid for role assignment");
        }

        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.AssignRoleToGroup(request.PrincipalId, request.GroupId);
        await _repository.AppendAsync(role, cancellationToken);
        return new AssignRoleToGroupCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}