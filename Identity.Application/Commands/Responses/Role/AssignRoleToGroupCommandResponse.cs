namespace Identity.Application.Commands.Responses.Role;

public record AssignRoleToGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);