using FluentValidation;
using Identity.Application.Commands.User;

namespace Identity.Application.Commands.Validators.User;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PictureUri).MaximumLength(500);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}