using MediatR;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Commands;

public class LogTimeTaskCommand : IRequest<LogTimeTaskCommandResponse>
{
    public Guid TenantId { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public ulong Duration { get; set; }
    public string Comment { get; set; }
    public DateOnly Day { get; set; }
}