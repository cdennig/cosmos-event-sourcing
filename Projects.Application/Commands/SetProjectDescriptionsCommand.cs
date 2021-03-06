using ES.Shared.Cache;
using Projects.Application.Commands.Responses;
using MediatR;

namespace Projects.Application.Commands;

public class SetProjectDescriptionsCommand : IRequest<SetProjectDescriptionsCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CacheKey => $"{TenantId}/project/{Id}";
}