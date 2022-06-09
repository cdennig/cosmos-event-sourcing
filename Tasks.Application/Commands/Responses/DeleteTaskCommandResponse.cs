namespace Tasks.Application.Commands.Responses;

public record DeleteTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
