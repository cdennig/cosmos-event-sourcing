using System;

namespace Projects.Application.Commands.Responses
{
    public record FinishProjectCommandResponse(Guid Id, long Version, string ResourceId);
}