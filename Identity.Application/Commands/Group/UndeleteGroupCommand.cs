﻿using Identity.Application.Commands.Responses.Group;
using MediatR;

namespace Identity.Application.Commands.Group;

public class UndeleteGroupCommand : IRequest<UndeleteGroupCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
}