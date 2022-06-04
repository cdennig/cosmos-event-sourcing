using FluentValidation;
using Identity.Application.Commands.Tenant;

namespace Identity.Application.Commands.Validators.Tenant;

public class UpdateLocationTenantCommandValidator : AbstractValidator<UpdateLocationTenantCommand>
{
    public UpdateLocationTenantCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(255);
    }
}