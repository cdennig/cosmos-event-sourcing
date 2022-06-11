using ES.Shared.Cache;
using Projects.Application.Commands.Responses;
using MediatR;

namespace Projects.Application.Commands;

public class SetProjectDatesCommand : IRequest<SetProjectDatesCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string CacheKey => $"{TenantId}/project/{Id}";
}