using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CleaningEventCreateRequestValidator : AbstractValidator<CleaningEventCreateRequest>
{
    public CleaningEventCreateRequestValidator()
    {
        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
        RuleFor(l => l.PollutedLocationId).NotNull();
    }
}
