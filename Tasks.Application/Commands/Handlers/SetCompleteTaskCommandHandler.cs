using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetCompleteTaskCommandHandler : IRequestHandler<SetCompleteTaskCommand, SetCompleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetCompleteTaskCommandHandler> _logger;

    public SetCompleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetCompleteTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetCompleteTaskCommandResponse> Handle(SetCompleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetComplete(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Completed task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        return new SetCompleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}