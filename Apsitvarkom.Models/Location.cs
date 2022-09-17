namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing locations.
/// </summary>
public class Location
{
    private double _latitude;
    private double _longitude;

    /// <summary>Latitude of the location. Cannot be less than -90.0 degrees or exceed 90.0 degrees.</summary>
    public double Latitude
    {
        get => _latitude;
        set
        {
            if (value is >= -90.0 and <= 90.0)
                _latitude = value;
            else
                throw new ArgumentOutOfRangeException(nameof(Latitude), $"Latitude cannot be less than -90.0 or exceed 90.0 degrees, but was {value}.");
        }
    }
    
    /// <summary>Longitude of the location. Cannot be less than -180 degrees or exceed 180 degrees.</summary>
    public double Longitude
    {
        get => _longitude;
        set
        {
            if (value is >= -180.0 and <= 180.0)
                _longitude = value;
            else
                throw new ArgumentOutOfRangeException(nameof(Longitude), $"Longitude cannot be less than -180.0 or exceed 180.0 degrees, but was {value}.");
        }
    }
}