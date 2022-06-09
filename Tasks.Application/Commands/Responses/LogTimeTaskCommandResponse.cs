namespace Tasks.Application.Commands.Responses;

public record LogTimeTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
