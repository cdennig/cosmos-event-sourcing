using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    RemoveFromProjectTaskCommandHandler : IRequestHandler<RemoveFromProjectTaskCommand,
        RemoveFromProjectTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<RemoveFromProjectTaskCommandHandler> _logger;

    public RemoveFromProjectTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<RemoveFromProjectTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RemoveFromProjectTaskCommandResponse> Handle(RemoveFromProjectTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        var project = task.ProjectId;
        _logger.LogInformation("Removing task {TaskId} from project {ProjectId} / tenant {TenantId}", request.Id,
            project, request.TenantId);
        task.RemoveFromProject(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Removed task {TaskId} from project {ProjectId} / tenant {TenantId}", request.Id,
            project, request.TenantId);
        return new RemoveFromProjectTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}