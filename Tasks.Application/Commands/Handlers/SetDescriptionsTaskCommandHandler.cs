using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetDescriptionsTaskCommandHandler : IRequestHandler<SetDescriptionsTaskCommand, SetDescriptionsTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetDescriptionsTaskCommandHandler> _logger;

    public SetDescriptionsTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetDescriptionsTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetDescriptionsTaskCommandResponse> Handle(SetDescriptionsTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new descriptions for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetDescriptions(request.PrincipalId, request.Title, request.Description);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("New descriptions set for task {TaskId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new SetDescriptionsTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}