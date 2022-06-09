using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class AssignToProjectTaskCommandValidator : AbstractValidator<AssignToProjectTaskCommand>
{
    public AssignToProjectTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
    }
}