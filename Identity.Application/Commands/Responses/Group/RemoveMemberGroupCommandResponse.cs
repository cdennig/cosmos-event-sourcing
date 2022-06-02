namespace Identity.Application.Commands.Responses.Group;

public record RemoveMemberGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);