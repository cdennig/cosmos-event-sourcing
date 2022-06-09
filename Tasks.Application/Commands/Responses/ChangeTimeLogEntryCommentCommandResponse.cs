namespace Tasks.Application.Commands.Responses;

public record ChangeTimeLogEntryCommentCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
