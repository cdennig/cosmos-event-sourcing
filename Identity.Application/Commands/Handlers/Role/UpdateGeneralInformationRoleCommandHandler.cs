using ES.Shared.Repository;
using Identity.Application.Commands.Handlers.Group;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Responses.Role;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Role;

public class UpdateGeneralInformationRoleCommandHandler : IRequestHandler<UpdateGeneralInformationRoleCommand,
    UpdateGeneralInformationRoleCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> _repository;
    private readonly ILogger<UpdateGeneralInformationGroupCommandHandler> _logger;

    public UpdateGeneralInformationRoleCommandHandler(
        ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> repository,
        ILogger<UpdateGeneralInformationGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateGeneralInformationRoleCommandResponse> Handle(UpdateGeneralInformationRoleCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating general information for role {RoleId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var role = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        role.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description);
        await _repository.AppendAsync(role, cancellationToken);
        _logger.LogInformation("Updated general information for role {RoleId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new UpdateGeneralInformationRoleCommandResponse(request.TenantId, role.Id, role.Version,
            role.ResourceId);
    }
}