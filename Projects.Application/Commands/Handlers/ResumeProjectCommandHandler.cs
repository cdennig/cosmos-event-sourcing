using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class ResumeProjectCommandHandler : IRequestHandler<ResumeProjectCommand, ResumeProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<ResumeProjectCommandHandler> _logger;

    public ResumeProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<ResumeProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ResumeProjectCommandResponse> Handle(ResumeProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resuming project {ProjectId} / tenant {TenantId}", request.Id, request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.ResumeProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("Project {ProjectId} / tenant {TenantId} resumed", request.Id, request.TenantId);
        return new ResumeProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}