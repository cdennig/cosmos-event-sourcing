using ES.Shared.Cache;
using MediatR;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands;

public class SetPriorityTaskCommand : IRequest<SetPriorityTaskCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public TaskPriority Priority { get; set; }
    public string CacheKey => $"{TenantId}/task/{Id}";
}