using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class SetProjectDatesCommandHandler : IRequestHandler<SetProjectDatesCommand,
        SetProjectDatesCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public SetProjectDatesCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<SetProjectDatesCommandResponse> Handle(SetProjectDatesCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.SetDates(request.StartDate, request.EndDate);
            await _repository.AppendAsync(p, cancellationToken);

            return new SetProjectDatesCommandResponse(p.Id, p.Version, p.ResourceId);
        }
    }
}