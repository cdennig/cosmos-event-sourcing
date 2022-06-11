using ES.Shared.Cache;
using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Group;

public class UpdateGeneralInformationGroupCommand : IRequest<UpdateGeneralInformationGroupCommandResponse>, IInvalidatesCacheCommand
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? PictureUri { get; set; }
    public string CacheKey => $"{TenantId}/group/{Id}";
}