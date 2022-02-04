namespace Identity.Application.Commands.Responses.Tenant;

public record SetPrimaryContactTenantCommandResponse(Guid Id, long Version, string ResourceId);