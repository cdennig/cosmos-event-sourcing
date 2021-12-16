namespace Projects.Application.Commands.Responses;

public record FinishProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);