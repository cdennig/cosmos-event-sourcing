namespace Tasks.Application.Commands.Responses;

public record SetPriorityTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
