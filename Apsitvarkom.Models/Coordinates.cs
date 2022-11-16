namespace Apsitvarkom.Models;

public class Coordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>Distance from source to destination coordinates using the Haversine formula.</summary>
    /// <param name="destination">Location to calculate distance to.</param>
    /// <returns>Distance from source to destination coordinates in meters.</returns>
    public double DistanceTo(Coordinates destination)
    {
        const double earthRadius = 6364050.0; // Calculated to Vilnius's latitude on ground level
        var deltaLat = (destination.Latitude - Latitude) * (Math.PI / 180.0);
        var deltaLon = (destination.Longitude - Longitude) * (Math.PI / 180.0);
        var havFunc = Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2) * Math.Cos(Latitude * (Math.PI / 180.0)) * Math.Cos(destination.Latitude * (Math.PI / 180.0))
                      + Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2);
        return 2 * earthRadius * Math.Asin(Math.Min(1, Math.Sqrt(havFunc)));
    }
}