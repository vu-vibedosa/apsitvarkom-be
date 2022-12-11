using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class PollutedLocationCreateRequestValidator : AbstractValidator<PollutedLocationCreateRequest>
{
    public PollutedLocationCreateRequestValidator(IValidator<LocationCreateRequest> locationCreateRequestValidator)
    {
        RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(l => l.Severity).NotNull();
        RuleFor(l => l.Location).NotNull().DependentRules(() =>
        {
            RuleFor(l => l.Location!).SetValidator(locationCreateRequestValidator);
        });
    }
}
