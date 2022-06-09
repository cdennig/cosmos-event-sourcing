using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    AssignToProjectTaskCommandHandler : IRequestHandler<AssignToProjectTaskCommand, AssignToProjectTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public AssignToProjectTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<AssignToProjectTaskCommandResponse> Handle(AssignToProjectTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.AssignToProject(request.PrincipalId, request.ProjectId);
        await _repository.AppendAsync(task, cancellationToken);

        return new AssignToProjectTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}