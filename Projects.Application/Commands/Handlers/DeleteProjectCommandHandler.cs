using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<DeleteProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteProjectCommandResponse> Handle(DeleteProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.DeleteProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} deleted / tenant {TenantId}", request.Id, request.TenantId);
        return new DeleteProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}