using MediatR;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands;

public class DeleteTimeLogEntryCommand : IRequest<DeleteTimeLogEntryCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public Guid EntryId { get; set; }
}