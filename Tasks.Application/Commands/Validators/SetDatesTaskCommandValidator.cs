using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class SetDatesTaskCommandValidator : AbstractValidator<SetDatesTaskCommand>
{
    public SetDatesTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.StartDate).NotNull().LessThan(ed => ed.EndDate);
        RuleFor(x => x.EndDate).NotNull().GreaterThan(sd => sd.StartDate);
    }
}