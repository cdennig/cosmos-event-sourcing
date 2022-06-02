namespace Identity.Application.Commands.Responses.Group;

public record UpdateGeneralInformationGroupCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);