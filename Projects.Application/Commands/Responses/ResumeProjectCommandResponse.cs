using System;

namespace Projects.Application.Commands.Responses
{
    public record ResumeProjectCommandResponse(Guid Id, long Version, string ResourceId);
}