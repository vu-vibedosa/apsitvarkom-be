using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CoordinatesGetResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CoordinatesGetResponseValidator : AbstractValidator<CoordinatesGetResponse>
{
    public CoordinatesGetResponseValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).InclusiveBetween(-180.0, 180.0);
    }
}