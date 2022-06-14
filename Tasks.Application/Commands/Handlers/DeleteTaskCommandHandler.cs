using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<DeleteTaskCommandHandler> _logger;

    public DeleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<DeleteTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteTaskCommandResponse> Handle(DeleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.DeleteTask(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Deleted task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        return new DeleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}