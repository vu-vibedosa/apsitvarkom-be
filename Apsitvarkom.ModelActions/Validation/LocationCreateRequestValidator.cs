using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class LocationCreateRequestValidator : AbstractValidator<LocationCreateRequest>
{
    public LocationCreateRequestValidator(IValidator<CoordinatesCreateRequest> coordinatesCreateRequestValidator)
    {
        RuleFor(l => l.Coordinates).NotNull();
        RuleFor(l => l.Coordinates!).SetValidator(coordinatesCreateRequestValidator).When(l => l.Coordinates is not null);
    }
}