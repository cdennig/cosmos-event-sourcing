using FluentValidation;

namespace Projects.Application.Commands.Validators;

public class SetProjectDatesCommandValidator : AbstractValidator<SetProjectDatesCommand>
{
    public SetProjectDatesCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.StartDate).NotNull().LessThan(x => x.EndDate);
        RuleFor(x => x.EndDate).NotNull().GreaterThan(x => x.StartDate);
    }
}