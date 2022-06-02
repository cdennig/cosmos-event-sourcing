using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Role;

public class RemoveRoleFromGroupCommandHandler : IRequestHandler<RemoveRoleFromGroupCommand,
    RemoveRoleFromGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly IGroupService _groupService;

    public RemoveRoleFromGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository, IGroupService groupService)
    {
        _repository = repository;
        _groupService = groupService;
    }

    public async Task<RemoveRoleFromGroupCommandResponse> Handle(RemoveRoleFromGroupCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _groupService.IsGroupValidForRoleAssignment(request.TenantId, request.GroupId))
        {
            throw new Exception("Group is not valid for role assignment");
        }

        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.RemoveRoleFromGroup(request.PrincipalId, request.GroupId);
        await _repository.AppendAsync(role, cancellationToken);
        return new RemoveRoleFromGroupCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}