using System;

namespace Projects.Application.Commands.Responses
{
    public record PauseProjectCommandResponse(Guid Id, long Version, string ResourceId);
}