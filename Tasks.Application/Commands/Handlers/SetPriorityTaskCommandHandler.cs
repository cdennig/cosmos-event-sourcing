using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetPriorityTaskCommandHandler : IRequestHandler<SetPriorityTaskCommand, SetPriorityTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetPriorityTaskCommandHandler> _logger;

    public SetPriorityTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetPriorityTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetPriorityTaskCommandResponse> Handle(SetPriorityTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new priority for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetPriority(request.PrincipalId, request.Priority);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("New priority set for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new SetPriorityTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}