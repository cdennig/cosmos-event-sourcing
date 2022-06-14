using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class UndeleteGroupCommandHandler : IRequestHandler<UndeleteGroupCommand,
    UndeleteGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly ILogger<UndeleteGroupCommandHandler> _logger;

    public UndeleteGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, ILogger<UndeleteGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UndeleteGroupCommandResponse> Handle(UndeleteGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Undeleting group {GroupId} / tenant {TenantId}", request.Id, request.TenantId);
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.UndeleteGroup(request.PrincipalId);
        await _repository.AppendAsync(group, cancellationToken);
        
        _logger.LogInformation("Undeleted group {GroupId} / tenant {TenantId}", request.Id, request.TenantId);
        
        return new UndeleteGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}