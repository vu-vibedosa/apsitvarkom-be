using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

public interface IGeocoder
{
    Task<string?> ReverseGeocodeAsync(Coordinates coordinates, string languageCode = "lt");

    Task<List<LocationTitle>> GetLocationTitlesAsync(Coordinates coordinates);
}