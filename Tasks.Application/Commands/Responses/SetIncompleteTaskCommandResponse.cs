namespace Tasks.Application.Commands.Responses;

public record SetIncompleteTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
