using System;

namespace Projects.Application.Commands.Responses
{
    public record StartProjectCommandResponse(Guid TenantId, Guid Id, long Version, string ResourceId);
}