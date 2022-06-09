using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetPriorityTaskCommandHandler : IRequestHandler<SetPriorityTaskCommand, SetPriorityTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetPriorityTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetPriorityTaskCommandResponse> Handle(SetPriorityTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetPriority(request.PrincipalId, request.Priority);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetPriorityTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}