using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class SetProjectPriorityCommandHandler : IRequestHandler<SetProjectPriorityCommand,
    SetProjectPriorityCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<SetProjectPriorityCommandHandler> _logger;

    public SetProjectPriorityCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<SetProjectPriorityCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetProjectPriorityCommandResponse> Handle(SetProjectPriorityCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new priority for project {0} / tenant {TenantId}", request.Id,
            request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.SetPriority(request.PrincipalId, request.Priority);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("New priority set for project {0} / tenant {TenantId}", request.Id, request.TenantId);
        return new SetProjectPriorityCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}