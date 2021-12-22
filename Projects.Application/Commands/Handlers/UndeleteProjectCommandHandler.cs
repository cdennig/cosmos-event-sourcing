using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class UndeleteProjectCommandHandler : IRequestHandler<UndeleteProjectCommand, UndeleteProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;

    public UndeleteProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UndeleteProjectCommandResponse> Handle(UndeleteProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.UndeleteProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        return new UndeleteProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}