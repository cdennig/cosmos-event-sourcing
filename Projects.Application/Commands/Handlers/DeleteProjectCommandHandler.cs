using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectCommandResponse>
{
    private readonly IEventsRepository<Guid, Project, Guid, Guid> _repository;

    public DeleteProjectCommandHandler(IEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<DeleteProjectCommandResponse> Handle(DeleteProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.DeleteProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        return new DeleteProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}