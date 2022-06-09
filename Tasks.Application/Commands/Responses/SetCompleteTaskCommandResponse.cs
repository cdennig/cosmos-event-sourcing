namespace Tasks.Application.Commands.Responses;

public record SetCompleteTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
