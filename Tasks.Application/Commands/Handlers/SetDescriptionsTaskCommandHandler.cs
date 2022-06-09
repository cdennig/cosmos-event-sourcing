using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetDescriptionsTaskCommandHandler : IRequestHandler<SetDescriptionsTaskCommand, SetDescriptionsTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetDescriptionsTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetDescriptionsTaskCommandResponse> Handle(SetDescriptionsTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetDescriptions(request.PrincipalId, request.Title, request.Description);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetDescriptionsTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}