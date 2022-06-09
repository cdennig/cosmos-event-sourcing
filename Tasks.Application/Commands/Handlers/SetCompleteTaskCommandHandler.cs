using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetCompleteTaskCommandHandler : IRequestHandler<SetCompleteTaskCommand, SetCompleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetCompleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetCompleteTaskCommandResponse> Handle(SetCompleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetComplete(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetCompleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}