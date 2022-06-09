using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands.Handlers;

public class
    ChangeTimeLogEntryCommentCommandHandler : IRequestHandler<ChangeTimeLogEntryCommentCommand,
        ChangeTimeLogEntryCommentCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public ChangeTimeLogEntryCommentCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<ChangeTimeLogEntryCommentCommandResponse> Handle(ChangeTimeLogEntryCommentCommand request,
        CancellationToken cancellationToken = default)
    {
        var task = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        task.ChangeTimeLogEntryComment(request.PrincipalId, request.EntryId, request.Comment);
        await _repository.AppendAsync(task, cancellationToken);

        return new ChangeTimeLogEntryCommentCommandResponse(task.TenantId, task.Id, task.Version, task.ResourceId);
    }
}