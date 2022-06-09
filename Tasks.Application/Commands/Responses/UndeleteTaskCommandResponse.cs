namespace Tasks.Application.Commands.Responses;

public record UndeleteTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
