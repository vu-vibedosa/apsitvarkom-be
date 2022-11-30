namespace Apsitvarkom.Models;

public class Location
{
    /// <summary>Location names in different languages.</summary>
    public List<LocationTitle> Titles { get; set; } = new();

    /// <summary>Longitude and Latitude of a location.</summary>
    public Coordinates Coordinates { get; set; } = new();
}