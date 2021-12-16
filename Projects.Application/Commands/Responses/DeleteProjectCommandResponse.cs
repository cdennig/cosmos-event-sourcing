namespace Projects.Application.Commands.Responses;

public record DeleteProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);