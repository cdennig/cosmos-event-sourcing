using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class PauseProjectCommandHandler : IRequestHandler<PauseProjectCommand, PauseProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<PauseProjectCommandHandler> _logger;

    public PauseProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<PauseProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PauseProjectCommandResponse> Handle(PauseProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Pausing project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.PauseProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} / tenant {TenantId} paused", request.Id, request.TenantId);
        return new PauseProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}