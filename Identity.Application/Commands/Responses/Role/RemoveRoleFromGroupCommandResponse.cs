namespace Identity.Application.Commands.Responses.Role;

public record RemoveRoleFromGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);