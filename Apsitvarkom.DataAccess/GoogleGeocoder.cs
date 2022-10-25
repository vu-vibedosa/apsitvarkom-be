using Apsitvarkom.Models;
using System.Text.Json;
using Yoh.Text.Json.NamingPolicies;

namespace Apsitvarkom.DataAccess;

public class GoogleGeocoder : IGeocoder
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicies.SnakeCaseLower
    };

    public GoogleGeocoder(HttpClient httpClient, IApiKeyProvider apiKey)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
    }

    public async Task<string?> ReverseGeocodeAsync(Coordinates coordinates)
    {
        var jsonStream = await _httpClient.GetStreamAsync($"?latlng={coordinates.Longitude},{coordinates.Latitude}&language=lt&result_type=political&key={_apiKey}");
        var response = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(jsonStream, SerializerOptions);

        var title = response?.Results?.FirstOrDefault()?.FormattedAddress;
        return title ?? string.Empty;
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
