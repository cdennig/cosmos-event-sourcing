using Identity.Application.Commands.Responses.Role;
using MediatR;

namespace Identity.Application.Commands.Role
{
    public class CreateRoleCommand : IRequest<CreateRoleCommandResponse>
    {
        public Guid TenantId { get; set; }
        public Guid PrincipalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBuiltIn { get; set; }
        public List<string> Actions { get; set; }
        public List<string> NotActions { get; set; }
    }
}