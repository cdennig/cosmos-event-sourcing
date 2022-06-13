using FluentValidation;

namespace Tasks.Application.Commands.Validators;

public class DeleteTimeLogEntryCommandValidator : AbstractValidator<DeleteTimeLogEntryCommand>
{
    public DeleteTimeLogEntryCommandValidator()
    {
        RuleFor(x => x.TenantId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.PrincipalId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Id).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.EntryId).NotNull().NotEqual(Guid.Empty);
    }
}