using ES.Shared.Cache;
using Projects.Application.Commands.Responses;
using Projects.Domain;
using MediatR;

namespace Projects.Application.Commands;

public class SetProjectPriorityCommand : IRequest<SetProjectPriorityCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public ProjectPriority Priority { get; set; }
    public string CacheKey => $"{TenantId}/project/{Id}";
}