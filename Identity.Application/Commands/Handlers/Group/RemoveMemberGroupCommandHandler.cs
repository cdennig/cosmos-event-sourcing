using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class RemoveMemberGroupCommandHandler : IRequestHandler<RemoveMemberGroupCommand,
    RemoveMemberGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly ILogger<RemoveMemberGroupCommandHandler> _logger;

    public RemoveMemberGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository,
        ILogger<RemoveMemberGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RemoveMemberGroupCommandResponse> Handle(RemoveMemberGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing member {MemberId} from group {GroupId} / tenant {TenantId}", request.MemberId,
            request.Id, request.TenantId);

        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.RemoveGroupMember(request.PrincipalId, request.MemberId);
        await _repository.AppendAsync(group, cancellationToken);

        _logger.LogInformation("Member {MemberId} removed from group {GroupId} / tenant {TenantId}", request.MemberId,
            request.Id, request.TenantId);

        return new RemoveMemberGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}