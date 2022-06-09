namespace Tasks.Application.Commands.Responses;

public record AssignToProjectTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
