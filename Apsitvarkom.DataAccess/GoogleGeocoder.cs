using System.Globalization;
using System.Text.Json;
using Apsitvarkom.Configuration;
using Apsitvarkom.Models;
using Yoh.Text.Json.NamingPolicies;

namespace Apsitvarkom.DataAccess;

public class GoogleGeocoder : IGeocoder
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicies.SnakeCaseLower
    };

    public GoogleGeocoder(HttpClient httpClient, IApiKeyProvider apiKeyProvider)
    {
        _apiKey = apiKeyProvider.Geocoding;
        _httpClient = httpClient;
    }

    public async Task<string?> ReverseGeocodeAsync(Coordinates coordinates, SupportedLanguages language)
    {
        var query = $"json?latlng={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)},{coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&language={Translated<string>.GetSupportedLocale(language)}&key={_apiKey}";
        var jsonStream = await _httpClient.GetStreamAsync(query);
        var response = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(jsonStream, SerializerOptions);
        return response?.Results?.FirstOrDefault()?.FormattedAddress;
    }

    public async Task<Translated<string>> ReverseGeocodeAsync(Coordinates coordinates)
    {
        var title = new Translated<string>();
        var languages = (SupportedLanguages[])Enum.GetValues(typeof(SupportedLanguages));

        var pairs = languages.Select(language => new { Language = language, Task = ReverseGeocodeAsync(coordinates, language) } ).ToList();
        await Task.WhenAll(pairs.Select(x => x.Task));

        foreach (var pair in pairs)
            title.Update(pair.Language, await pair.Task ?? string.Empty);
        
        return title;
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