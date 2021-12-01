using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class StartProjectCommandHandler : IRequestHandler<StartProjectCommand, StartProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public StartProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<StartProjectCommandResponse> Handle(StartProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.StartProject();
            await _repository.AppendAsync(p, cancellationToken);

            return new StartProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}