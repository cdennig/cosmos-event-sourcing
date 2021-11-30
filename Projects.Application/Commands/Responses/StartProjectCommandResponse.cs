using System;

namespace Projects.Application.Commands.Responses
{
    public record StartProjectCommandResponse(Guid Id, long Version, string ResourceId);
}