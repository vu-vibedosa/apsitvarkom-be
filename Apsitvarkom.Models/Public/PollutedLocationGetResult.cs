using System.Globalization;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationGetResult
{
    public string Id { get; init; } = string.Empty;

    public int Radius { get; set; }

    public string Severity { get; set; } = PollutedLocation.SeverityLevel.Low.ToString();

    public string Spotted { get; init; } = DateTime.MinValue.ToString("o", CultureInfo.InvariantCulture);

    public int Progress { get; set; }

    public string? Notes { get; set; }

    public LocationGetResult Location { get; set; } = new();
}
