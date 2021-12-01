using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class SetProjectPriorityCommandHandler : IRequestHandler<SetProjectPriorityCommand,
        SetProjectPriorityCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid, Guid> _repository;

        public SetProjectPriorityCommandHandler(IEventsRepository<Guid, Project, Guid, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<SetProjectPriorityCommandResponse> Handle(SetProjectPriorityCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.SetPriority(request.PrincipalId, request.Priority);
            await _repository.AppendAsync(p, cancellationToken);

            return new SetProjectPriorityCommandResponse(p.TenantId, p.Id, p.Version, p.ResourceId);
        }
    }
}