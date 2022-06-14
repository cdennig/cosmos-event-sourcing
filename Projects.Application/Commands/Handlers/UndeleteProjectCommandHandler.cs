using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class UndeleteProjectCommandHandler : IRequestHandler<UndeleteProjectCommand, UndeleteProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<UndeleteProjectCommandHandler> _logger;

    public UndeleteProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<UndeleteProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UndeleteProjectCommandResponse> Handle(UndeleteProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Undeleting project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.UndeleteProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} / tenant {TenantId} undeleted", request.Id, request.TenantId);
        return new UndeleteProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}