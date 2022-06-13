using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class SetTimeEstimationTaskCommandValidator : AbstractValidator<SetTimeEstimationTaskCommand>
{
    public SetTimeEstimationTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Estimation).LessThan(525600UL); // one year in minutes
    }
}