namespace Apsitvarkom.Models.Public;

public class CleaningEventResponse
{
    public Guid Id { get; init; }

    public Guid PollutedLocationId { get; set; }

    public DateTime StartTime { get; set; }

    public string? Notes { get; set; }
}