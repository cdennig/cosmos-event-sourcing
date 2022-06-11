using ES.Shared.Cache;
using Projects.Application.Commands.Responses;
using MediatR;

namespace Projects.Application.Commands;

public class DeleteProjectCommand : IRequest<DeleteProjectCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string CacheKey => $"{TenantId}/project/{Id}";
}