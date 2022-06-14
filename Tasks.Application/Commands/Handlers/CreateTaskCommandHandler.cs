using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<CreateTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateTaskCommandResponse> Handle(CreateTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating task with properties: {@CreateTaskCommand}", request);

        var task = Domain.Task.Initialize(request.TenantId, request.PrincipalId,
            Guid.NewGuid(), request.Title,
            request.Description, request.ProjectId,
            request.StartDate, request.EndDate, request.Priority ?? TaskPriority.Medium);

        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Task created {@TaskId} / tenant {TenantId}", task.Id, task.TenantId);
        return new CreateTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}