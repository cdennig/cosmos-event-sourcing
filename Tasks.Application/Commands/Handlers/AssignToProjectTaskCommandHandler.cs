using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    AssignToProjectTaskCommandHandler : IRequestHandler<AssignToProjectTaskCommand, AssignToProjectTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<AssignToProjectTaskCommandHandler> _logger;

    public AssignToProjectTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<AssignToProjectTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AssignToProjectTaskCommandResponse> Handle(AssignToProjectTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Assigning task {TaskId} to project {ProjectID} / tenant {TenantID}", request.Id,
            request.ProjectId, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.AssignToProject(request.PrincipalId, request.ProjectId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Task {TaskId} assigned to project {ProjectID} / tenant {TenantID}", request.Id,
            request.ProjectId, request.TenantId);
        return new AssignToProjectTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}