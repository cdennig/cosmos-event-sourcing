namespace Identity.Application.Commands.Responses.User;

public record UpdateEmailUserCommandResponse(Guid Id, long Version, string ResourceId);