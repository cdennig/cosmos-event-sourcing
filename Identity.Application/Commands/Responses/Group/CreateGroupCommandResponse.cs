namespace Identity.Application.Commands.Responses.Group;

public record CreateGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);