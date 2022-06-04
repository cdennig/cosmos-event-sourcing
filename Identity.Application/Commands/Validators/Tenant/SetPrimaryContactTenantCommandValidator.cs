using FluentValidation;
using Identity.Application.Commands.Tenant;

namespace Identity.Application.Commands.Validators.Tenant;

public class SetPrimaryContactTenantCommandValidator : AbstractValidator<SetPrimaryContactTenantCommand>
{
    public SetPrimaryContactTenantCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.ContactId).NotNull().NotEqual(Guid.Empty);
    }
}