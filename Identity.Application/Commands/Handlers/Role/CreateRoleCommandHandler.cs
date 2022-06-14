using ES.Shared.Repository;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Role;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateRoleCommandResponse> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new role with properties {@CreateRoleCommand}", request);

        var role = Domain.Role.Initialize(request.TenantId, request.PrincipalId, Guid.NewGuid(), request.Name,
            request.Actions, request.NotActions, request.Description, request.IsBuiltIn);
        await _repository.AppendAsync(role, cancellationToken);
        _logger.LogInformation("Created new role {RoleId} / tenant {TenantId}", role.Id, role.TenantId);
        return new CreateRoleCommandResponse(request.TenantId, role.Id, role.Version, role.ResourceId);
    }
}