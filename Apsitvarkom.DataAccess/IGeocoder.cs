using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

public interface IGeocoder
{
    Task<List<LocationTitle>> ReverseGeocodeAsync(Coordinates coordinates);
}