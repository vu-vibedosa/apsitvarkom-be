namespace Apsitvarkom.Models;

public class Location
{
    /// <summary>Location name in different languages.</summary>
    public Translated<string> Title { get; set; } = new();

    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; } = new();
}