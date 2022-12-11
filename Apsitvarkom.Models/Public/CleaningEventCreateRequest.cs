namespace Apsitvarkom.Models.Public;

public class CleaningEventCreateRequest
{
    public Guid? PollutedLocationId { get; set; }

    public DateTime? StartTime { get; set; }

    public string? Notes { get; set; }
}