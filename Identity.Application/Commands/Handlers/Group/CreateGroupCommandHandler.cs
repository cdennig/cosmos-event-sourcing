using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, CreateGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;

    public CreateGroupCommandHandler(ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateGroupCommandResponse> Handle(CreateGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = Domain.Group.Initialize(request.TenantId, request.PrincipalId, Guid.NewGuid(), request.Name,
            request.Description, request.PictureUri);
        await _repository.AppendAsync(group, cancellationToken);
        return new CreateGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}