using System;

namespace Projects.Application.Commands.Responses
{
    public record CreateProjectCommandResponse(Guid Id, long Version, string ResourceId);
}