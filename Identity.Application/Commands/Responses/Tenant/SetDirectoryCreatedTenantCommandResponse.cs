namespace Identity.Application.Commands.Responses.Tenant;

public record SetDirectoryCreatedTenantCommandResponse(Guid Id, long Version, string ResourceId);