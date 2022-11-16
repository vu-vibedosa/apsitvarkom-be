using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class LocationGetResponse
{
    // TODO: Title that will be populated by the geocoding api should be inserted here

    public CoordinatesGetResponse Coordinates { get; set; } = new();
}

public class LocationGetResponseValidator : AbstractValidator<LocationGetResponse>
{
    public LocationGetResponseValidator(IValidator<CoordinatesGetResponse> coordinatesGetResponseValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesGetResponseValidator);
    }
}