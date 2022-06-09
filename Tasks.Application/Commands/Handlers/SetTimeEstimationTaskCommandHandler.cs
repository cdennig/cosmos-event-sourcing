using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetTimeEstimationTaskCommandHandler : IRequestHandler<SetTimeEstimationTaskCommand,
        SetTimeEstimationTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetTimeEstimationTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetTimeEstimationTaskCommandResponse> Handle(SetTimeEstimationTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetTimeEstimation(request.PrincipalId, request.Estimation);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetTimeEstimationTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}