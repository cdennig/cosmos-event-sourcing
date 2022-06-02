namespace Identity.Application.Commands.Responses.Group;

public record AddMemberGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);