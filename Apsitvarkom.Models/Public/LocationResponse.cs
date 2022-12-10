namespace Apsitvarkom.Models.Public;

public class LocationResponse
{
    public CoordinatesResponse Coordinates { get; set; } = new();
    public TranslatedResponse<string> Title { get; set; } = new();
}