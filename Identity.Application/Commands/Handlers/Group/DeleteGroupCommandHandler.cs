using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand,
    DeleteGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly ILogger<DeleteGroupCommandHandler> _logger;

    public DeleteGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository, ILogger<DeleteGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<DeleteGroupCommandResponse> Handle(DeleteGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting group {GroupId} / tenant {TenantId}", request.Id, request.TenantId);

        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.DeleteGroup(request.PrincipalId);
        await _repository.AppendAsync(group, cancellationToken);

        _logger.LogInformation("Deleted group {GroupId} / tenant {TenantId}", request.Id, request.TenantId);

        return new DeleteGroupCommandResponse(request.TenantId, group.Id, group.Version, group.ResourceId);
    }
}