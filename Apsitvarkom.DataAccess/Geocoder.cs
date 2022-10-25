using Apsitvarkom.Models;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Apsitvarkom.DataAccess;

public class Geocoder : IGeocoder
{
    const string GeocodingApiEndpoint = "https://maps.googleapis.com/maps/api/geocode/json";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public Geocoder(HttpClient httpClient, string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = httpClient;
    }

    public async Task<string?> ReverseGeocodeAsync(Coordinates coordinates)
    {
        var jsonStream = await _httpClient.GetStreamAsync($"{GeocodingApiEndpoint}?latlng={coordinates.Longitude},{coordinates.Latitude}&language=lt&result_type=political&key={_apiKey}");
        var deserializedApiResponse = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(jsonStream);
        var formattedTitle = deserializedApiResponse?.Results?.FirstOrDefault()?.FormattedAddress;
        return formattedTitle ?? string.Empty;
    }

    private class ReverseGeocodingApiResult
    {
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }
    }

    private class ReverseGeocodingApiResponse
    {
        [JsonPropertyName("results")]
        public IEnumerable<ReverseGeocodingApiResult>? Results { get; set; }
    }
}
