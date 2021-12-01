using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class UndeleteProjectCommandHandler : IRequestHandler<UndeleteProjectCommand, UndeleteProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public UndeleteProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<UndeleteProjectCommandResponse> Handle(UndeleteProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.UndeleteProject();
            await _repository.AppendAsync(p, cancellationToken);

            return new UndeleteProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}