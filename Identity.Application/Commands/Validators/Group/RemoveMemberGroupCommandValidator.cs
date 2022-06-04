using FluentValidation;
using Identity.Application.Commands.Group;

namespace Identity.Application.Commands.Validators.Group;

public class RemoveMemberGroupCommandValidator : AbstractValidator<RemoveMemberGroupCommand>
{
    public RemoveMemberGroupCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.MemberId).NotNull().NotEqual(Guid.Empty);
    }
}