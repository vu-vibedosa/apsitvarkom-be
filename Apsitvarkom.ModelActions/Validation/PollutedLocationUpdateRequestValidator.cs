using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class PollutedLocationUpdateRequestValidator : AbstractValidator<PollutedLocationUpdateRequest>
{
    public PollutedLocationUpdateRequestValidator()
    {
        RuleFor(l => l.Id).NotNull();
        RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(l => l.Severity).NotNull();
    }
}