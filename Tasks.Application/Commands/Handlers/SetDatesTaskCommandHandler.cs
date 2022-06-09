using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetDatesTaskCommandHandler : IRequestHandler<SetDatesTaskCommand, SetDatesTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public SetDatesTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetDatesTaskCommandResponse> Handle(SetDatesTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetDates(request.PrincipalId, request.StartDate, request.EndDate);
        await _repository.AppendAsync(task, cancellationToken);

        return new SetDatesTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}