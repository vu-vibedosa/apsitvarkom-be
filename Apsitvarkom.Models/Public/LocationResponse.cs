using FluentValidation;
using static Apsitvarkom.Models.LocationTitle;

namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    public CoordinatesResponse Coordinates { get; set; } = new();
    public List<LocationTitleResponse> Title { get; set; } = new();
}

public class LocationResponseValidator : AbstractValidator<LocationResponse>
{
    public LocationResponseValidator(IValidator<CoordinatesResponse> coordinatesResponseValidator, IValidator<LocationTitleResponse> locationTitleResponseValidator)
    {
        RuleFor(location => location.Coordinates).SetValidator(coordinatesResponseValidator);
        RuleForEach(location => location.Title).SetValidator(locationTitleResponseValidator);
        RuleFor(location => location.Title.Count).Equal(Enum.GetValues(typeof(LocationCode)).Length);
    }
}