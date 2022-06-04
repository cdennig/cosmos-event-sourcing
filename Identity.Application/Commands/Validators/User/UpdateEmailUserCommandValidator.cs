using FluentValidation;
using Identity.Application.Commands.User;

namespace Identity.Application.Commands.Validators.User;

public class UpdateEmailUserCommandValidator : AbstractValidator<UpdateEmailUserCommand>
{
    public UpdateEmailUserCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Email).NotNull().EmailAddress();
    }
}