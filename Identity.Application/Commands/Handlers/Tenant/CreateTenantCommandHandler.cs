using ES.Shared.Repository;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Tenant;
using MediatR;

namespace Identity.Application.Commands.Handlers.Tenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantCommandResponse>
{
    private readonly IEventsRepository<Domain.Tenant, Guid, Guid> _repository;

    public CreateTenantCommandHandler(IEventsRepository<Domain.Tenant, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<CreateTenantCommandResponse> Handle(CreateTenantCommand request,
        CancellationToken cancellationToken = default)
    {
        var user = Domain.Tenant.Initialize(request.PrincipalId, Guid.NewGuid(), request.Name, request.Description,
            request.Language, request.Location ?? string.Empty, request.PictureUri ?? string.Empty);
        await _repository.AppendAsync(user, cancellationToken);
        return new CreateTenantCommandResponse(user.Id, user.Version, user.ResourceId);
    }
}