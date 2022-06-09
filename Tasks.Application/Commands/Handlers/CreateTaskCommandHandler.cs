using ES.Shared.Repository;
using MediatR;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> _repository;

    public CreateTaskCommandHandler(ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateTaskCommandResponse> Handle(CreateTaskCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = Domain.Task.Initialize(request.TenantId, request.PrincipalId,
            Guid.NewGuid(), request.Title,
            request.Description, request.ProjectId,
            request.StartDate, request.EndDate, request.Priority ?? TaskPriority.Medium);

        await _repository.AppendAsync(p, cancellationToken);

        return new CreateTaskCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}