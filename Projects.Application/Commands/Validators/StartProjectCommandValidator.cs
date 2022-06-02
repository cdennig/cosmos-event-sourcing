using FluentValidation;

namespace Projects.Application.Commands.Validators;

public class StartProjectCommandValidator : AbstractValidator<StartProjectCommand>
{
    public StartProjectCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
    }
}