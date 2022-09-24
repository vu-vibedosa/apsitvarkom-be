using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing captured polluted location records.
/// </summary>
public class PollutedLocation
{
    /// <summary>Unique identifier of the given record.</summary>
    public Guid Id { get; init; }

    /// <summary>Geolocation of the given record.</summary>
    public Location Location { get; set; } = null!;

    /// <summary>Rough size estimate of the given record area in meters from the center.</summary>
    public int Radius { get; set; }

    /// <summary>Estimated current pollution level of the record.</summary>
    public LocationSeverityLevel Severity { get; set; }

    /// <summary><see cref="DateTime" /> of when the record was created.</summary>
    public DateTime Spotted { get; init; }

    /// <summary>Current progress of the record's cleaning process in percentages.</summary>
    public int Progress { get; set; }

    /// <summary>Additional information about the record.</summary>
    public string Notes { get; set; } = null!;
}