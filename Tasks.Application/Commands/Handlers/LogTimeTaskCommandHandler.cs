using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    LogTimeTaskCommandHandler : IRequestHandler<LogTimeTaskCommand,
        LogTimeTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<LogTimeTaskCommandHandler> _logger;

    public LogTimeTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<LogTimeTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<LogTimeTaskCommandResponse> Handle(LogTimeTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Logging time for task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.LogTime(request.PrincipalId, request.Duration, request.Comment, request.Day);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Time logged for task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        return new LogTimeTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}