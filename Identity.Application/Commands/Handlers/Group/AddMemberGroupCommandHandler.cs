using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class AddMemberGroupCommandHandler : IRequestHandler<AddMemberGroupCommand,
    AddMemberGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly IUserService _userService;
    
    public AddMemberGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, IUserService userService)
    {
        _repository = repository;
        _userService= userService;
    }

    public async Task<AddMemberGroupCommandResponse> Handle(AddMemberGroupCommand request,
        CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserValidAsGroupMember(request.MemberId))
        {
            throw new Exception("User is not valid as group member");
        }
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.AddGroupMember(request.PrincipalId, request.MemberId);
        await _repository.AppendAsync(group, cancellationToken);
        return new AddMemberGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}