namespace Apsitvarkom.Models;

/// <summary>
/// Class for extensions used in the Models project.
/// </summary>
public static class Extensions
{
    /// <summary>Distance from source to destination coordinates using the Haversine formula.</summary>
    /// <param name="source">Location to calculate distance from.</param>
    /// <param name="destination">Location to calculate distance to.</param>
    /// <returns>Distance from source to destination coordinates in meters.</returns>
    public static double DistanceTo(this Location source, Location destination)
    {
        const double earthRadius = 6364050.0; // Calculated to Vilnius's latitude on ground level
        var deltaLat = (destination.Coordinates.Latitude - source.Coordinates.Latitude) * (Math.PI / 180.0);
        var deltaLon = (destination.Coordinates.Longitude - source.Coordinates.Longitude) * (Math.PI / 180.0);
        var havFunc = Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2) * Math.Cos(source.Coordinates.Latitude * (Math.PI / 180.0)) * Math.Cos(destination.Coordinates.Latitude * (Math.PI / 180.0))
                      + Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2);
        return 2 * earthRadius * Math.Asin(Math.Min(1, Math.Sqrt(havFunc)));
    }
}