using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public CreateProjectCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = Project.Initialize(request.TenantId, Guid.NewGuid(), request.Title, request.Description,
                request.StartDate, request.EndDate, request.Priority);
            await _repository.AppendAsync(p, cancellationToken);

            return new CreateProjectCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}