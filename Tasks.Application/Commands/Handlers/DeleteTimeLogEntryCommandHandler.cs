using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    DeleteTimeLogEntryCommandHandler : IRequestHandler<DeleteTimeLogEntryCommand, DeleteTimeLogEntryCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public DeleteTimeLogEntryCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<DeleteTimeLogEntryCommandResponse> Handle(DeleteTimeLogEntryCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.DeleteTimeLogEntry(request.PrincipalId, request.EntryId);
        await _repository.AppendAsync(task, cancellationToken);

        return new DeleteTimeLogEntryCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}