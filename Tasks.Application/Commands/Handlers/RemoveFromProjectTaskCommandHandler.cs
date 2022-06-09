using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    RemoveFromProjectTaskCommandHandler : IRequestHandler<RemoveFromProjectTaskCommand, RemoveFromProjectTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public RemoveFromProjectTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<RemoveFromProjectTaskCommandResponse> Handle(RemoveFromProjectTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.RemoveFromProject(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        return new RemoveFromProjectTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}