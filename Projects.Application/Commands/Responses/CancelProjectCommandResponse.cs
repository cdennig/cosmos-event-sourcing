using System;

namespace Projects.Application.Commands.Responses
{
    public record CancelProjectCommandResponse(Guid Id, long Version, string ResourceId);
}