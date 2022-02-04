namespace Identity.Application.Commands.Responses.User;

public record DeleteUserCommandResponse(Guid Id, long Version, string ResourceId);