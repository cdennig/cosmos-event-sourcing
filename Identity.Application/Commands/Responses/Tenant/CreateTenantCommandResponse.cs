namespace Identity.Application.Commands.Responses.Tenant;

public record CreateTenantCommandResponse(Guid Id, long Version, string ResourceId);