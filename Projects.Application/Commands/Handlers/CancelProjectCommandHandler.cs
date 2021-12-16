using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class CancelProjectCommandHandler : IRequestHandler<CancelProjectCommand, CancelProjectCommandResponse>
{
    private readonly IEventsRepository<Guid, Project, Guid, Guid> _repository;

    public CancelProjectCommandHandler(IEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CancelProjectCommandResponse> Handle(CancelProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.CancelProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        return new CancelProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}