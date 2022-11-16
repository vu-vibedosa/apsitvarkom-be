using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CoordinatesGetRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CoordinatesGetRequestValidator : AbstractValidator<CoordinatesGetRequest>
{
    public CoordinatesGetRequestValidator()
    {
        // Is .NotNull() needed here?
        RuleFor(dto => dto.Latitude).NotNull().InclusiveBetween(-90.0, 90.0);
        RuleFor(dto => dto.Longitude).NotNull().InclusiveBetween(-180.0, 180.0);
    }
}