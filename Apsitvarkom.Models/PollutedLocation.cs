using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing captured polluted location records.
/// </summary>
public class PollutedLocation
{
    private int _progress;
    private int _radius;

    /// <summary>Unique identifier of the given record.</summary>
    public string Id { get; init; } = null!;

    /// <summary>Geolocation of the given record.</summary>
    public Location Location { get; set; } = null!;

    /// <summary>Rough size estimate of the given record area in meters from the center.</summary>
    public int Radius
    {
        get => _radius;
        set
        {
            if (value >= 1)
                _radius = value;
            else
                throw new ArgumentOutOfRangeException(nameof(Radius), "Radius can only be higher than 0.");
        }
    }

    /// <summary>Estimated current pollution level of the record.</summary>
    public LocationSeverityLevel Severity { get; set; }

    /// <summary><see cref="DateTime" /> of when the record was created.</summary>
    public DateTime Spotted { get; init; }

    /// <summary>Current progress of the record's cleaning process in percentages.</summary>
    public int Progress
    {
        get => _progress;
        set
        {
            if (value is >= 0 and <= 100)
                _progress = value;
            else 
                throw new ArgumentOutOfRangeException(nameof(Progress),
                    "Progress is depicted in percentages and can only have values between 0 and 100.");
        }
    }

    /// <summary>Additional information about the record.</summary>
    public string Notes { get; set; } = null!;
}