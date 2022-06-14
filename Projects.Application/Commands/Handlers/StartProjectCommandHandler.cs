using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class StartProjectCommandHandler : IRequestHandler<StartProjectCommand, StartProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<StartProjectCommandHandler> _logger;

    public StartProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<StartProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<StartProjectCommandResponse> Handle(StartProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);

        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.StartProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project started {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        return new StartProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}