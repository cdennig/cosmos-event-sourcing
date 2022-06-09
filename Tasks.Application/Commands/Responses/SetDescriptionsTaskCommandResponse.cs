namespace Tasks.Application.Commands.Responses;

public record SetDescriptionsTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
