﻿using FluentValidation;

namespace Apsitvarkom.Models;

/// <summary>
/// Struct used for storing coordinates of a polluted location.
/// </summary>
public struct Coordinates
{
    public double Latitude;
    public double Longitude;
}

public class CoordinatesValidator : AbstractValidator<Coordinates>
{
    public CoordinatesValidator()
    {
        RuleFor(coordinates => coordinates.Longitude).InclusiveBetween(-180.0, 180.0);
        RuleFor(coordinates => coordinates.Latitude).InclusiveBetween(-90.0, 90.0);
    }
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
public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(data => data.Coordinates).SetValidator(new CoordinatesValidator());
    }
}