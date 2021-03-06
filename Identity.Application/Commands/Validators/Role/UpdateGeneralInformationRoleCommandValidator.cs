using FluentValidation;
using Identity.Application.Commands.Role;

namespace Identity.Application.Commands.Validators.Role;

public class UpdateGeneralInformationRoleCommandValidator : AbstractValidator<UpdateGeneralInformationRoleCommand>
{
    public UpdateGeneralInformationRoleCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}