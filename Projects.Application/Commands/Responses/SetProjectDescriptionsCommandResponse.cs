namespace Projects.Application.Commands.Responses;

public record SetProjectDescriptionsCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);