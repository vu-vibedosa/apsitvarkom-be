namespace Apsitvarkom.Models;

public class LocationTitle
{
    public enum LocationCode
    {
        en,
        lt
    }

    public LocationCode Code { get; set; }

    public string Name { get; set; } = string.Empty;
}

