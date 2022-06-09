namespace Tasks.Application.Commands.Responses;

public record DeleteTimeLogEntryCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
