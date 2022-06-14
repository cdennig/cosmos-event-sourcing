using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, CreateGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly ILogger<CreateGroupCommandHandler> _logger;

    public CreateGroupCommandHandler(ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository,
        ILogger<CreateGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateGroupCommandResponse> Handle(CreateGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new group with properties {@CreateGroupCommand}", request);

        var group = Domain.Group.Initialize(request.TenantId, request.PrincipalId, Guid.NewGuid(), request.Name,
            request.Description, request.PictureUri);
        await _repository.AppendAsync(group, cancellationToken);

        _logger.LogInformation("Created new group {GroupId} / tenant {TenantId}", group.Id, group.TenantId);

        return new CreateGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}