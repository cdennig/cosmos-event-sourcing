namespace Identity.Application.Commands.Responses.User;

public record CreateUserCommandResponse(Guid Id, long Version, string ResourceId);