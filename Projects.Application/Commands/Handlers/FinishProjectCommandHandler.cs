using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class FinishProjectCommandHandler : IRequestHandler<FinishProjectCommand, FinishProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public FinishProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<FinishProjectCommandResponse> Handle(FinishProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.FinishProject();
            await _repository.AppendAsync(p, cancellationToken);

            return new FinishProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}