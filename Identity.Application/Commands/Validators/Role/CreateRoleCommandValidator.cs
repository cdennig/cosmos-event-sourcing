using FluentValidation;
using Identity.Application.Commands.Role;

namespace Identity.Application.Commands.Validators.Role;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
        When(x => x.Actions.Count == 0, () => { RuleFor(x => x.NotActions).NotEmpty(); });
        When(x => x.NotActions.Count == 0, () => { RuleFor(x => x.Actions).NotEmpty(); });
    }
}