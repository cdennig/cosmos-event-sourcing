using ES.Shared.Repository;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Responses.Group;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Handlers.Group;

public class UpdateGeneralInformationGroupCommandHandler : IRequestHandler<UpdateGeneralInformationGroupCommand,
    UpdateGeneralInformationGroupCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> _repository;
    private readonly ILogger<UpdateGeneralInformationGroupCommandHandler> _logger;

    public UpdateGeneralInformationGroupCommandHandler(
        ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> repository,
        ILogger<UpdateGeneralInformationGroupCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpdateGeneralInformationGroupCommandResponse> Handle(UpdateGeneralInformationGroupCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating general information of group {GroupId} / tenant {TenantId}", request.Id,
            request.TenantId);
        var group = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        group.UpdateGeneralInformation(request.PrincipalId, request.Name, request.Description,
            request.PictureUri ?? "");
        await _repository.AppendAsync(group, cancellationToken);

        _logger.LogInformation("Updated general information of group {GroupId} / tenant {TenantId}", request.Id,
            request.TenantId);

        return new UpdateGeneralInformationGroupCommandResponse(request.TenantId, group.Id, group.Version,
            group.ResourceId);
    }
}