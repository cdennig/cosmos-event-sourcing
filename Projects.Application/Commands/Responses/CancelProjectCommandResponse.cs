using System;

namespace Projects.Application.Commands.Responses
{
    public record CancelProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
}