namespace Identity.Application.Commands.Responses.Role;

public record UpdateGeneralInformationRoleCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);