using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    UndeleteTaskCommandHandler : IRequestHandler<UndeleteTaskCommand, UndeleteTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<UndeleteTaskCommandHandler> _logger;

    public UndeleteTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<UndeleteTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UndeleteTaskCommandResponse> Handle(UndeleteTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Undeleting task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.UndeleteTask(request.PrincipalId);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Undeleted task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        return new UndeleteTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}