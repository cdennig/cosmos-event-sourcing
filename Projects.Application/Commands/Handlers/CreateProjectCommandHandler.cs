using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;

    public CreateProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = Project.Initialize(request.TenantId, request.PrincipalId,
            Guid.NewGuid(), request.Title,
            request.Description,
            request.StartDate, request.EndDate, request.Priority);
        
        await _repository.AppendAsync(p, cancellationToken);

        return new CreateProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}