using ES.Shared.Cache;
using MediatR;
using Tasks.Application.Commands.Responses;

namespace Tasks.Application.Commands;

public class SetDatesTaskCommand : IRequest<SetDatesTaskCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string CacheKey => $"{TenantId}/task/{Id}";
}