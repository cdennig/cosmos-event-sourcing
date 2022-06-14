using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetTimeEstimationTaskCommandHandler : IRequestHandler<SetTimeEstimationTaskCommand,
        SetTimeEstimationTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetTimeEstimationTaskCommandHandler> _logger;

    public SetTimeEstimationTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetTimeEstimationTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetTimeEstimationTaskCommandResponse> Handle(SetTimeEstimationTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting time estimation for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetTimeEstimation(request.PrincipalId, request.Estimation);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("Time estimation set for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new SetTimeEstimationTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}