using ES.Shared.Cache;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class LogTimeTaskCommand : IRequest<LogTimeTaskCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public ulong Duration { get; set; }
    public string Comment { get; set; }
    public DateOnly Day { get; set; }
    public string CacheKey => $"{TenantId}/task/{Id}";
}