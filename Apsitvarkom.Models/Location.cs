using FluentValidation;

namespace Apsitvarkom.Models;

/// <summary>
/// Class used for storing coordinates of a polluted location.
/// </summary>
public class Coordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CoordinatesValidator : AbstractValidator<Coordinates>
{
    public CoordinatesValidator()
    {
        RuleFor(coordinates => coordinates.Latitude).InclusiveBetween(-90.0, 90.0);
        RuleFor(coordinates => coordinates.Longitude).InclusiveBetween(-180.0, 180.0);
    }
}

/// <summary>
/// Class for storing locations.
/// </summary>
public class Location
{
    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; } = null!;
}

/// <summary>
/// Class for validating location properties.
/// </summary>
public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator(CoordinatesValidator coordinatesValidator)
    {
        RuleFor(data => data.Coordinates).SetValidator(coordinatesValidator);
    }
}