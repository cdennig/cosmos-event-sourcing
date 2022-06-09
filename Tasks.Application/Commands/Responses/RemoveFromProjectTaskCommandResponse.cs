namespace Tasks.Application.Commands.Responses;

public record RemoveFromProjectTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
