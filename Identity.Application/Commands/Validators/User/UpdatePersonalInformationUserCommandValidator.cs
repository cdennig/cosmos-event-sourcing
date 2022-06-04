using FluentValidation;
using Identity.Application.Commands.User;

namespace Identity.Application.Commands.Validators.User;

public class UpdatePersonalInformationUserCommandValidator : AbstractValidator<UpdatePersonalInformationUserCommand>
{
    public UpdatePersonalInformationUserCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PictureUri).MaximumLength(500);
    }
}