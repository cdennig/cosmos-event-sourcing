using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class UndeleteGroupCommandHandler : IRequestHandler<UndeleteGroupCommand,
    UndeleteGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly IUserService _userService;
    
    public UndeleteGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, IUserService userService)
    {
        _repository = repository;
        _userService= userService;
    }

    public async Task<UndeleteGroupCommandResponse> Handle(UndeleteGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.UndeleteGroup(request.PrincipalId);
        await _repository.AppendAsync(group, cancellationToken);
        return new UndeleteGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}