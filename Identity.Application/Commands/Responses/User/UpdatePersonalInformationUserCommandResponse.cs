namespace Identity.Application.Commands.Responses.User;

public record UpdatePersonalInformationUserCommandResponse(Guid Id, long Version, string ResourceId);