using ES.Shared.Cache;
using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Group;

public class RemoveMemberGroupCommand : IRequest<RemoveMemberGroupCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string CacheKey => $"{TenantId}/group/{Id}";
}