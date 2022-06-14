using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    ChangeTimeLogEntryCommentCommandHandler : IRequestHandler<ChangeTimeLogEntryCommentCommand,
        ChangeTimeLogEntryCommentCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;
    private readonly ILogger<ChangeTimeLogEntryCommentCommandHandler> _logger;

    public ChangeTimeLogEntryCommentCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository,
        ILogger<ChangeTimeLogEntryCommentCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ChangeTimeLogEntryCommentCommandResponse> Handle(ChangeTimeLogEntryCommentCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Changing time log entry comment for task {TaskId} / entry {EntryId} / tenant {TenantId}",
            request.Id, request.EntryId, request.TenantId);
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.ChangeTimeLogEntryComment(request.PrincipalId, request.EntryId, request.Comment);
        await _repository.AppendAsync(task, cancellationToken);

        _logger.LogInformation(
            "Changed time log entry comment for task {TaskId} / entry id {EntryId} / tenant {TenantId}",
            request.Id, request.EntryId, request.TenantId);
        return new ChangeTimeLogEntryCommentCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}