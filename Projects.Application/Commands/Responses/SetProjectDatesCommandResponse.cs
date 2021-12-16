namespace Projects.Application.Commands.Responses;

public record SetProjectDatesCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);