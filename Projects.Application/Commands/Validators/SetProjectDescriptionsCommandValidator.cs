using FluentValidation;

namespace Projects.Application.Commands.Validators;

public class SetProjectDescriptionsCommandValidator : AbstractValidator<SetProjectDescriptionsCommand>
{
    public SetProjectDescriptionsCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}