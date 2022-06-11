using ES.Shared.Cache;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class SetIncompleteTaskCommand : IRequest<SetIncompleteTaskCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string CacheKey => $"{TenantId}/task/{Id}";
}