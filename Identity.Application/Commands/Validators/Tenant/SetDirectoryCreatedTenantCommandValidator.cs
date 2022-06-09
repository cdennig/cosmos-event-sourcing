using FluentValidation;
using Identity.Application.Commands.Tenant;

namespace Identity.Application.Commands.Validators.Tenant;

public class SetDirectoryCreatedTenantCommandValidator : AbstractValidator<SetDirectoryCreatedTenantCommand>
{
    public SetDirectoryCreatedTenantCommandValidator()
    {
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.AdminGroupId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.UsersGroupId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.AdminRoleId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.UsersRoleId).NotNull().NotEqual(Guid.Empty);
    }
}