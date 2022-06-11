using ES.Shared.Cache;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class ChangeTimeLogEntryCommentCommand : IRequest<ChangeTimeLogEntryCommentCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid EntryId { get; set; }
    public string Comment { get; set; }
    public string CacheKey => $"{TenantId}/task/{Id}";
}