using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CoordinatesResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CoordinatesResponseValidator : AbstractValidator<CoordinatesResponse>
{
    public CoordinatesResponseValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).InclusiveBetween(-180.0, 180.0);
    }
}