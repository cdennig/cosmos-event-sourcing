namespace Identity.Application.Commands.Responses.User;

public record ConfirmUserCommandResponse(Guid Id, long Version, string ResourceId);