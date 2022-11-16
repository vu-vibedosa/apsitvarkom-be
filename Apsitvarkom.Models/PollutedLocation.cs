namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing captured polluted location records.
/// </summary>
public class PollutedLocation
{
    public enum SeverityLevel
    {
        Low = 1,
        Moderate = 2,
        High = 3
    }

    /// <summary>Unique identifier of the given record.</summary>
    public Guid Id { get; init; }

    /// <summary>Rough size estimate of the given record area in meters from the center.</summary>
    public int Radius { get; set; }

    /// <summary>Estimated current pollution level of the record.</summary>
    public SeverityLevel Severity { get; set; }

    /// <summary><see cref="DateTime" /> of when the record was created.</summary>
    public DateTime Spotted { get; init; }

    /// <summary>Current progress of the record's cleaning process in percentages.</summary>
    public int Progress { get; set; }

    /// <summary>Additional information about the record.</summary>
    public string? Notes { get; set; }

    /// <summary>Geographic location information about the record.</summary>
    public Location Location { get; set; } = new();
}