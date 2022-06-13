using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class SetIncompleteTaskCommandValidator : AbstractValidator<SetIncompleteTaskCommand>
{
    public SetIncompleteTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
    }
}