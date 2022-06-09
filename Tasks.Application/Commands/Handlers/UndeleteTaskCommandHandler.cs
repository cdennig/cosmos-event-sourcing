using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    UndeleteTaskCommandHandler : IRequestHandler<UndeleteTaskCommand, UndeleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public UndeleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UndeleteTaskCommandResponse> Handle(UndeleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.UndeleteTask(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        return new UndeleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}