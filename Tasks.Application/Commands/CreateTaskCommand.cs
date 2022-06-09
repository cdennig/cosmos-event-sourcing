using MediatR;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands;

public class CreateTaskCommand : IRequest<CreateTaskCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public TaskPriority? Priority { get; set; }
    public ulong TimeEstimation { get; set; }
}