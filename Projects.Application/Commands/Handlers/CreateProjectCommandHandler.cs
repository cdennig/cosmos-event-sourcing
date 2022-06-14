using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating project with properties {@CreateProjectCommand}", request);
        var p = Project.Initialize(request.TenantId, request.PrincipalId,
            Guid.NewGuid(), request.Title,
            request.Description,
            request.StartDate, request.EndDate, request.Priority);

        await _repository.AppendAsync(p, cancellationToken);
        _logger.LogInformation("Project created: {ProjectId} / tenant {TenantId}", p.Id, p.TenantId);
        return new CreateProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}