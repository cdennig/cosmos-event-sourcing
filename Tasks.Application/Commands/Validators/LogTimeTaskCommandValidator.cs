using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class LogTimeTaskCommandValidator : AbstractValidator<LogTimeTaskCommand>
{
    public LogTimeTaskCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Comment).NotNull().MaximumLength(2000);
        RuleFor(x => x.Day).NotNull();
        RuleFor(x => x.Duration).LessThan(1440UL);
    }
}