namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="Location" />
/// </summary>
public class LocationDTO
{
    /// <summary>Property equivalent to <see cref="Location.Longitude" /></summary>
    public double? Longitude { get; set; } = null!;

    /// <summary>Property equivalent to <see cref="Location.Latitude" /></summary>
    public double? Latitude { get; set; } = null!;
}