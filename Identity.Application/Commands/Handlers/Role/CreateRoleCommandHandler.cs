using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using MediatR;

namespace Identity.Application.Commands.Handlers.Role;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;

    public CreateRoleCommandHandler(ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateRoleCommandResponse> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = Domain.Role.Initialize(request.TenantId, request.PrincipalId, Guid.NewGuid(), request.Name,
            request.Actions, request.NotActions, request.Description, request.IsBuiltIn);
        await _repository.AppendAsync(role, cancellationToken);
        return new CreateRoleCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}