using FluentValidation;
using Identity.Application.Commands.User;

namespace Identity.Application.Commands.Validators.User;

public class UndeleteUserCommandValidator : AbstractValidator<UndeleteUserCommand>
{
    public UndeleteUserCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
    }
}