using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class PollutedLocationResponseValidator : AbstractValidator<PollutedLocationResponse>
{
    public PollutedLocationResponseValidator(IValidator<LocationResponse> locationResponseValidator)
    {
        RuleFor(l => l.Location).SetValidator(locationResponseValidator);
        RuleFor(l => l.Radius).GreaterThanOrEqualTo(1);
        RuleFor(l => l.Progress).InclusiveBetween(0, 100);
    }
}
