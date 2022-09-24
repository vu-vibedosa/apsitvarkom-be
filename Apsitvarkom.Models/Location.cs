namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing locations.
/// </summary>
public class Location
{
    /// <summary>Latitude of the location. Cannot be less than -90.0 degrees or exceed 90.0 degrees.</summary>
    public double Latitude { get; set; }
    
    /// <summary>Longitude of the location. Cannot be less than -180 degrees or exceed 180 degrees.</summary>
    public double Longitude { get; set; }
}