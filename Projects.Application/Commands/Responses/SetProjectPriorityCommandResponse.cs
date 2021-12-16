namespace Projects.Application.Commands.Responses;

public record SetProjectPriorityCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);