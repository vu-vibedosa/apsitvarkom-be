namespace Apsitvarkom.Models;

public class Location
{
    // TODO: Title that will be populated by the geocoding api goes here

    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; } = new();
}