using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class ResumeProjectCommandHandler : IRequestHandler<ResumeProjectCommand, ResumeProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;

    public ResumeProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<ResumeProjectCommandResponse> Handle(ResumeProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.ResumeProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        return new ResumeProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}