namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing records of events for cleaning of specific <see cref="PollutedLocation" />.
/// </summary>
public class CleaningEvent
{
    /// <summary>Unique identifier of the given event.</summary>
    public Guid Id { get; init; }

    /// <summary>Unique identifier of the <see cref="Models.PollutedLocation" /> linked to the event.</summary>
    public Guid PollutedLocationId { get; set; }

    /// <summary><see cref="DateTime" /> of when the event is due to start.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Additional information about the event.</summary>
    public string? Notes { get; set; }

    /// <summary>Specifies whether the results for the event have already been submitted.</summary>
    public bool IsFinalized { get; set; }
}