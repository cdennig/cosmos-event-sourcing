using Identity.Application.Commands.Responses.Tenant;
using MediatR;

namespace Identity.Application.Commands.Tenant;

public class UpdateLanguageTenantCommand : IRequest<UpdateLanguageTenantCommandResponse>
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string Language { get; set; }
}