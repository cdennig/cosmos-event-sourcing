using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Services;
using MediatR;

namespace Identity.Application.Commands.Handlers.Group;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand,
    DeleteGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly IUserService _userService;
    
    public DeleteGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, IUserService userService)
    {
        _repository = repository;
        _userService= userService;
    }

    public async Task<DeleteGroupCommandResponse> Handle(DeleteGroupCommand request,
        CancellationToken cancellationToken)
    {
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.DeleteGroup(request.PrincipalId);
        await _repository.AppendAsync(group, cancellationToken);
        return new DeleteGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}