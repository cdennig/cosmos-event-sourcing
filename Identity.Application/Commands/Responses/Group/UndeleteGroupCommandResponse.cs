namespace Identity.Application.Commands.Responses.Group;

public record UndeleteGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);