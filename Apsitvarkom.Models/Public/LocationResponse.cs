using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    // TODO: Title that will be populated by the geocoding api should be inserted here

    public CoordinatesResponse Coordinates { get; set; } = new();
}

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
    }
}