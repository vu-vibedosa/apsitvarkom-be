namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="PollutedLocation" />
/// </summary>
public class PollutedLocationDTO : PollutedLocationDTOBase
{
    /// <summary>Property equivalent to <see cref="PollutedLocation.Id" /></summary>
    public string Id { get; init; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Spotted" /></summary>
    public string Spotted { get; init; }
}