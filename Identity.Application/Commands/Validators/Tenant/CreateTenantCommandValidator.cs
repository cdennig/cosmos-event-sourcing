using FluentValidation;
using Identity.Application.Commands.Tenant;

namespace Identity.Application.Commands.Validators.Tenant;

public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.PictureUri).MaximumLength(500);
        RuleFor(x => x.Language).NotEmpty();
    }
}