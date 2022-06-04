using FluentValidation;
using Identity.Application.Commands.Role;

namespace Identity.Application.Commands.Validators.Role;

public class RemoveRoleFromGroupCommandValidator : AbstractValidator<RemoveRoleFromGroupCommand>
{
    public RemoveRoleFromGroupCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.GroupId).NotNull().NotEqual(Guid.Empty);
    }
}