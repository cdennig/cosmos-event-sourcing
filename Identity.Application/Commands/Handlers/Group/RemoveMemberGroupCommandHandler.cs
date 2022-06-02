using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class RemoveMemberGroupCommandHandler : IRequestHandler<RemoveMemberGroupCommand,
    RemoveMemberGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly IUserService _userService;
    
    public RemoveMemberGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, IUserService userService)
    {
        _repository = repository;
        _userService= userService;
    }

    public async Task<RemoveMemberGroupCommandResponse> Handle(RemoveMemberGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.RemoveGroupMember(request.PrincipalId, request.MemberId);
        await _repository.AppendAsync(group, cancellationToken);
        return new RemoveMemberGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}