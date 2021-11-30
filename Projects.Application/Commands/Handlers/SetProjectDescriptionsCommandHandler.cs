using System;
using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Repository;
using MediatR;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Commands.Handlers
{
    public class SetProjectDescriptionsCommandHandler : IRequestHandler<SetProjectDescriptionsCommand,
        SetProjectDescriptionsCommandResponse>
    {
        private readonly IEventsRepository<Guid, Project, Guid> _repository;

        public SetProjectDescriptionsCommandHandler(IEventsRepository<Guid, Project, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<SetProjectDescriptionsCommandResponse> Handle(SetProjectDescriptionsCommand request,
            CancellationToken cancellationToken = default)
        {
            var p = await _repository.RehydrateAsync(request.TenantId, request.Id, cancellationToken);
            p.SetDescriptions(request.Title, request.Description);
            await _repository.AppendAsync(p, cancellationToken);

            return new SetProjectDescriptionsCommandResponse(p.Id, p.Version, p.ResourceId);
        }
    }
}