using Apsitvarkom.Models.Public;
using FluentValidation;

namespace Apsitvarkom.ModelActions.Validation;

public class CoordinatesResponseValidator : AbstractValidator<CoordinatesResponse>
{
    public CoordinatesResponseValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).InclusiveBetween(-180.0, 180.0);
    }
}