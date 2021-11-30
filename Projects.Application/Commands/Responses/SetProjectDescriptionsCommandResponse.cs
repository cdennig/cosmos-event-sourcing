using System;

namespace Projects.Application.Commands.Responses
{
    public record SetProjectDescriptionsCommandResponse(Guid Id, long Version, string ResourceId);
}