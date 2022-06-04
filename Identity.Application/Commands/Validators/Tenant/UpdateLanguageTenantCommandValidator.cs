using FluentValidation;
using Identity.Application.Commands.Tenant;

namespace Identity.Application.Commands.Validators.Tenant;

public class UpdateLanguageTenantCommandValidator : AbstractValidator<UpdateLanguageTenantCommand>
{
    public UpdateLanguageTenantCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Language).NotEmpty();
    }
}