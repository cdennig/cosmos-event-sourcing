using FluentValidation;
using Identity.Application.Commands.Group;

namespace Identity.Application.Commands.Validators.Group;

public class UpdateGeneralInformationGroupCommandValidator : AbstractValidator<UpdateGeneralInformationGroupCommand>
{
    public UpdateGeneralInformationGroupCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PictureUri).MaximumLength(500);
    }
}