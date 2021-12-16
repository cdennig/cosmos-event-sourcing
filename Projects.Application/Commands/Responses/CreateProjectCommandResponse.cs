namespace Projects.Application.Commands.Responses;

public record CreateProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);