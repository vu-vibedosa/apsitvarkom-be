namespace Apsitvarkom.Models;

public class Location
{
    /// <summary>Name of the location.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; } = new();
}