namespace Tasks.Application.Commands.Responses;

public record SetTimeEstimationTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
