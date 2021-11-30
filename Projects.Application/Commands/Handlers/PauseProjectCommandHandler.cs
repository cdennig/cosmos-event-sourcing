using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class PauseProjectCommandHandler : IRequestHandler<PauseProjectCommand, PauseProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public PauseProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<PauseProjectCommandResponse> Handle(PauseProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.PauseProject();
            await _repository.AppendAsync(p, cancellationToken);

            return new PauseProjectCommandResponse(p.Id, p.Version, p.ResourceId);
        }
    }
}