using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class RemoveFromProjectTaskCommandValidator : AbstractValidator<RemoveFromProjectTaskCommand>
{
    public RemoveFromProjectTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
    }
}