using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(4000);
        // EndDate may be empty, if not, StartDate must have a valid value
        When(x => x.EndDate.HasValue, () => { RuleFor(x => x.StartDate).NotEmpty().LessThan(x => x.EndDate); });
        When(x => x.ProjectId.HasValue, () => { RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty); });
    }
}