namespace Identity.Application.Commands.Responses.Role;

public record CreateRoleCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);