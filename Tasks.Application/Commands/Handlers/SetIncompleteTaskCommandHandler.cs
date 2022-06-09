using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetIncompleteTaskCommandHandler : IRequestHandler<SetIncompleteTaskCommand, SetIncompleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetIncompleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetIncompleteTaskCommandResponse> Handle(SetIncompleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetIncomplete(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetIncompleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}