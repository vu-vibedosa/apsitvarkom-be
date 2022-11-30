using Apsitvarkom.Configuration;
using Apsitvarkom.Models;
using System.Globalization;
using System.Text.Json;
using Yoh.Text.Json.NamingPolicies;
using static Apsitvarkom.Models.LocationTitle;

namespace Apsitvarkom.DataAccess;

public class GoogleGeocoder : IGeocoder
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private static readonly List<LocationCode> _languages = Enum.GetValues(typeof(LocationCode)).Cast<LocationCode>().ToList();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicies.SnakeCaseLower
    };

    public GoogleGeocoder(HttpClient httpClient, IApiKeyProvider apiKeyProvider)
    {
        _apiKey = apiKeyProvider.Geocoding;
        _httpClient = httpClient;
    }

    public async Task<List<LocationTitle>> ReverseGeocodeAsync(Coordinates coordinates)
    {
        var result = new List<LocationTitle>();
        foreach (var languageCode in _languages)
        {
            var query = $"json?latlng={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)},{coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&language={languageCode}&key={_apiKey}";
            var jsonStream = await _httpClient.GetStreamAsync(query);
            var response = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(jsonStream, SerializerOptions);
            result.Add(new() { Code = languageCode, Name = response?.Results?.FirstOrDefault()?.FormattedAddress ?? string.Empty });
        }
        return result;
    }

    private class ReverseGeocodingApiResult
    {
        public string? FormattedAddress { get; set; }
    }

    private class ReverseGeocodingApiResponse
    {
        public IEnumerable<ReverseGeocodingApiResult>? Results { get; set; }
    }
}
