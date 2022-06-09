namespace Tasks.Application.Commands.Responses;

public record CreateTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
