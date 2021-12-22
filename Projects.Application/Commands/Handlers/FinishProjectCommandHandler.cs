﻿using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers;

public class FinishProjectCommandHandler : IRequestHandler<FinishProjectCommand, FinishProjectCommandResponse>
{
    private readonly ITenantEventsRepository<Guid, Project, Guid, Guid> _repository;

    public FinishProjectCommandHandler(ITenantEventsRepository<Guid, Project, Guid, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<FinishProjectCommandResponse> Handle(FinishProjectCommand request,
        CancellationToken cancellationToken = default)
    {
        var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
        p.FinishProject(request.PrincipalId);
        await _repository.AppendAsync(p, cancellationToken);

        return new FinishProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
    }
}