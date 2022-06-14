using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    DeleteTimeLogEntryCommandHandler : IRequestHandler<DeleteTimeLogEntryCommand, DeleteTimeLogEntryCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<DeleteTimeLogEntryCommandHandler> _logger;

    public DeleteTimeLogEntryCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<DeleteTimeLogEntryCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteTimeLogEntryCommandResponse> Handle(DeleteTimeLogEntryCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting time log entry {EntryId} for task {TaskId} / tenant {TenantId}",
            request.EntryId, request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.DeleteTimeLogEntry(request.PrincipalId, request.EntryId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Deleted time log entry {EntryId} for task {TaskId} / tenant {TenantId}",
            request.EntryId, request.Id, request.TenantId);
        return new DeleteTimeLogEntryCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}