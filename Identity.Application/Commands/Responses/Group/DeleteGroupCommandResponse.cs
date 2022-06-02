namespace Identity.Application.Commands.Responses.Group;

public record DeleteGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);