using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetIncompleteTaskCommandHandler : IRequestHandler<SetIncompleteTaskCommand, SetIncompleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetIncompleteTaskCommandHandler> _logger;

    public SetIncompleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetIncompleteTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetIncompleteTaskCommandResponse> Handle(SetIncompleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Set task {TaskId} / tenant {TenantId} to incomplete", request.Id,
            request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetIncomplete(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Task {TaskId} / tenant {TenantId} set to incomplete", request.Id,
            request.TenantId);
        return new SetIncompleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}