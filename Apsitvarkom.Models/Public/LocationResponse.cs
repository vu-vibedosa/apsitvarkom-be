using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    public string Title { get; set; } = string.Empty;
    public CoordinatesResponse Coordinates { get; set; } = new();
}

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
    }
}