using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    SetDatesTaskCommandHandler : IRequestHandler<SetDatesTaskCommand, SetDatesTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<SetDatesTaskCommandHandler> _logger;

    public SetDatesTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<SetDatesTaskCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetDatesTaskCommandResponse> Handle(SetDatesTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new dates for task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.SetDates(request.PrincipalId, request.StartDate, request.EndDate);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation("New dates set for task {TaskId} / tenant {TenantId}", request.Id, request.TenantId);
        return new SetDatesTaskCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}