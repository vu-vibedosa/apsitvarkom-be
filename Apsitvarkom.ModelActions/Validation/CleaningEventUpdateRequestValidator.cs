using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventUpdateRequestValidator : AbstractValidator<CleaningEventUpdateRequest>
{
    public CleaningEventUpdateRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
    }
}