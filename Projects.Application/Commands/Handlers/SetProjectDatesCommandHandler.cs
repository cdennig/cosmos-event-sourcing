using ES.Shared.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class SetProjectDatesCommandHandler : IRequestHandler<SetProjectDatesCommand,
    SetProjectDatesCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;
    private readonly ILogger<SetProjectDatesCommandHandler> _logger;

    public SetProjectDatesCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository,
        ILogger<SetProjectDatesCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SetProjectDatesCommandResponse> Handle(SetProjectDatesCommand request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting new project dates for project {ProjectId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.SetDates(request.PrincipalId, request.StartDate, request.EndDate);
        await _repository.AppendAsync(p, cancellationToken);

        _logger.LogInformation("New dates set for project {ProjectId} / tenant {TenantId}", request.Id,
            request.TenantId);
        return new SetProjectDatesCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}