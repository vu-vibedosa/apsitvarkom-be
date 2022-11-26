namespace Apsitvarkom.Models.Public;

public class TidyingEventResponse
{
    public Guid Id { get; init; }

    public Guid PollutedLocationId { get; set; }

    public DateTime StartTime { get; set; }

    public string? Notes { get; set; }
}