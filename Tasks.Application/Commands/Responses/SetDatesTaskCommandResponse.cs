namespace Tasks.Application.Commands.Responses;

public record SetDatesTaskCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
