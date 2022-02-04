namespace Identity.Application.Commands.Responses.User;

public record UndeleteUserCommandResponse(Guid Id, long Version, string ResourceId);