using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CoordinatesCreateRequestValidator : AbstractValidator<CoordinatesCreateRequest>
{
    public CoordinatesCreateRequestValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).NotNull().InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).NotNull().InclusiveBetween(-180.0, 180.0);
    }
}