using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class ResumeProjectCommandHandler : IRequestHandler<ResumeProjectCommand, ResumeProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public ResumeProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<ResumeProjectCommandResponse> Handle(ResumeProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.ResumeProject();
            await _repository.AppendAsync(p, cancellationToken);

            return new ResumeProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}