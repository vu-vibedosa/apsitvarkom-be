using static Apsitvarkom.Models.LocationTitle;

namespace Apsitvarkom.Models.Public;

public class LocationTitleResponse
{
    public LocationCode Code { get; set; }
    public string Name { get; set; } = string.Empty;
}