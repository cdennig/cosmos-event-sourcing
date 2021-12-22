using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class SetProjectDescriptionsCommandHandler : IRequestHandler<SetProjectDescriptionsCommand,
    SetProjectDescriptionsCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;

    public SetProjectDescriptionsCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<SetProjectDescriptionsCommandResponse> Handle(SetProjectDescriptionsCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.SetDescriptions(request.PrincipalId, request.Title, request.Description);
        await _repository.AppendAsync(p, cancellationToken);

        return new SetProjectDescriptionsCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}