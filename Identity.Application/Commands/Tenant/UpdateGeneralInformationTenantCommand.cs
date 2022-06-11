using ES.Shared.Cache;
using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class UpdateGeneralInformationTenantCommand : IRequest<UpdateGeneralInformationTenantCommandResponse>,
    IInvalidatesCacheCommand
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PictureUri { get; set; }
    public string CacheKey => $"tenant/{Id}";
}