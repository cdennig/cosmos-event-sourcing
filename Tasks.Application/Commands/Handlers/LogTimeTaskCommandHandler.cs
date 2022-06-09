using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    LogTimeTaskCommandHandler : IRequestHandler<LogTimeTaskCommand,
        LogTimeTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public LogTimeTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<LogTimeTaskCommandResponse> Handle(LogTimeTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.LogTime(request.PrincipalId, request.Duration, request.Comment, request.Day);
        await _repository.AppendAsync(task, cancellationToken);

        return new LogTimeTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}