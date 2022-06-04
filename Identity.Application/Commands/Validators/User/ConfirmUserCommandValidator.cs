using FluentValidation;
using Identity.Application.Commands.User;

namespace Identity.Application.Commands.Validators.User;

public class ConfirmUserCommandValidator : AbstractValidator<ConfirmUserCommand>
{
    public ConfirmUserCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
    }
}