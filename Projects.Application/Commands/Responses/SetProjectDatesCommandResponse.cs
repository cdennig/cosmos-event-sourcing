using System;

namespace Projects.Application.Commands.Responses
{
    public record SetProjectDatesCommandResponse(Guid Id, long Version, string ResourceId);
}