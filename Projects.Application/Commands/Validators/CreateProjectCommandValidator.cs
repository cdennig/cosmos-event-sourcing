using FluentValidation;

namespace Projects.Application.Commands.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        
        // EndDate may be empty, if not, StartDate must have a valid value
        When(x => x.EndDate.HasValue, () =>
        {
            RuleFor(x => x.StartDate).NotEmpty().LessThan(x => x.EndDate);
        });
    }
}