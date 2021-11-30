using System;

namespace Projects.Application.Commands.Responses
{
    public record SetProjectPriorityCommandResponse(Guid Id, long Version, string ResourceId);
}