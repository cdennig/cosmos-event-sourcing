using ES.Shared.Cache;
using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class SetPrimaryContactTenantCommand : IRequest<SetPrimaryContactTenantCommandResponse>, IInvalidatesCacheCommand
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string CacheKey => $"tenant/{Id}";
}