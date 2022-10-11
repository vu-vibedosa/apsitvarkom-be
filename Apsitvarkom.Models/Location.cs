using FluentValidation;

namespace Apsitvarkom.Models;

/// <summary>
/// Struct used for storing coordinates of a polluted location.
/// </summary>
public struct Coordinates
{
    public double Latitude;
    public double Longitude;
}

/// <summary>
/// Class for storing locations.
/// </summary>
public class Location
{
    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; }
}

/// <summary>
/// Class for validating location properties.
/// </summary>
public class LocationValidator : AbstractValidator<Coordinates>
{
    public LocationValidator()
    {
        RuleFor(data => data.Longitude).InclusiveBetween(-180.0, 180.0);
        RuleFor(data => data.Latitude).InclusiveBetween(-90.0, 90.0);
    }
}