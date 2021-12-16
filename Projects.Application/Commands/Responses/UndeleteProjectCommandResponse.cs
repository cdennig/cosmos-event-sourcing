namespace Projects.Application.Commands.Responses;

public record UndeleteProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);