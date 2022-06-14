using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class SetProjectDescriptionsCommandHandler : IRequestHandler<SetProjectDescriptionsCommand,
    SetProjectDescriptionsCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<SetProjectDescriptionsCommandHandler> _logger;

    public SetProjectDescriptionsCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<SetProjectDescriptionsCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetProjectDescriptionsCommandResponse> Handle(SetProjectDescriptionsCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new project descriptions for project {ProjectId} / tenant {TenantId}",
            request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.SetDescriptions(request.PrincipalId, request.Title, request.Description);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("New project descriptions set for project {ProjectId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new SetProjectDescriptionsCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}