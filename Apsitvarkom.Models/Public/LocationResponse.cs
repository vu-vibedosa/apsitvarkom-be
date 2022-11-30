using FluentValidation;
using static Apsitvarkom.Models.LocationTitle;

namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    public CoordinatesResponse Coordinates { get; set; } = new();
    public List<LocationTitleResponse> Titles { get; set; } = new();
}

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
        RuleFor(location => location.Titles.Count).Equal(Enum.GetValues(typeof(LocationCode)).Length);
    }
}