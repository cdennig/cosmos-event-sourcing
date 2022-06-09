using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, DeleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public DeleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<DeleteTaskCommandResponse> Handle(DeleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.DeleteTask(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        return new DeleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}