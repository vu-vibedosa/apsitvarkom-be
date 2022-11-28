namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing records of events for cleaning of specific <see cref="PollutedLocation" />.
/// </summary>
public class CleaningEvent
{
    /// <summary>Unique identifier of the given event.</summary>
    public Guid Id { get; init; }

    /// <summary>Unique identifier of the <see cref="PollutedLocation" /> linked to the event.</summary>
    public Guid PollutedLocationId { get; set; }

    /// <summary><see cref="DateTime" /> of when the event is due to start.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>Additional information about the event.</summary>
    public string? Notes { get; set; }
}