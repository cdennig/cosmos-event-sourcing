using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class SetDescriptionsTaskCommandValidator : AbstractValidator<SetDescriptionsTaskCommand>
{
    public SetDescriptionsTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(4000);
    }
}