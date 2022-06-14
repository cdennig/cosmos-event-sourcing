using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class FinishProjectCommandHandler : IRequestHandler<FinishProjectCommand, FinishProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<FinishProjectCommandHandler> _logger;

    public FinishProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<FinishProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<FinishProjectCommandResponse> Handle(FinishProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Finishing project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.FinishProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} / tenant {TenantId} finished", request.Id, request.TenantId);
        return new FinishProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}