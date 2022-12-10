using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationUpdateRequest
{
    public Guid? Id { get; set; }

    public int? Radius { get; set; }

    public SeverityLevel? Severity { get; set; }

    public string? Notes { get; set; }
}