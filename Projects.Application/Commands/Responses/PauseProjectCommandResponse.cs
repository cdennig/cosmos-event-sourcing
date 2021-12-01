using System;

namespace Projects.Application.Commands.Responses
{
    public record PauseProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
}