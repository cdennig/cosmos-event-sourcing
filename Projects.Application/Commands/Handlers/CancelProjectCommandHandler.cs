using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class CancelProjectCommandHandler : IRequestHandler<CancelProjectCommand, CancelProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<CancelProjectCommandHandler> _logger;

    public CancelProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<CancelProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CancelProjectCommandResponse> Handle(CancelProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.CancelProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} cancelled / tenant {TenantId}", request.Id, request.TenantId);
        return new CancelProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}