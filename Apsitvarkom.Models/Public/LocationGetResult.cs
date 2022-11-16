namespace Apsitvarkom.Models.Public;

public class LocationGetResult
{
    // TODO: Title that will be populated by the geocoding api should be inserted here

    public CoordinatesGetResult Coordinates { get; set; } = new();
}