namespace Projects.Application.Commands.Responses;

public record ResumeProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);