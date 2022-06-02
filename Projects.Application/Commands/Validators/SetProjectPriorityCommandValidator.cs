using FluentValidation;

namespace Projects.Application.Commands.Validators;

public class SetProjectPriorityCommandValidator : AbstractValidator<SetProjectPriorityCommand>
{
    public SetProjectPriorityCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Priority).IsInEnum();
    }
}