using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class AddMemberGroupCommandHandler : IRequestHandler<AddMemberGroupCommand,
    AddMemberGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly IUserService _userService;
    private readonly ILogger<AddMemberGroupCommandHandler> _logger;

    public AddMemberGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, IUserService userService,
        ILogger<AddMemberGroupCommandHandler> logger)
    {
        _repository = repository;
        _userService = userService;
        _logger = logger;
    }

    public async Task<AddMemberGroupCommandResponse> Handle(AddMemberGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding member {MemberId} to group {GroupId} / tenant {TenantId}", request.MemberId,
            request.Id, request.TenantId);
        if (!await _userService.IsUserValidAsGroupMember(request.MemberId))
        {
            _logger.LogWarning("Member {MemberId} not valid as group member", request.MemberId);
            throw new ArgumentException("User is not valid as group member");
        }

        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.AddGroupMember(request.PrincipalId, request.MemberId);
        await _repository.AppendAsync(group, cancellationToken);

        _logger.LogInformation("Member {MemberId} added to group {GroupId} / tenant {TenantId}", request.MemberId,
            request.Id, request.TenantId);
        return new AddMemberGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}