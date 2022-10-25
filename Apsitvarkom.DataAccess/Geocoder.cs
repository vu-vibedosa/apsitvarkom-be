using Apsitvarkom.Models;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Apsitvarkom.DataAccess
{
    public class Geocoder : IGeocoder
    {
        private readonly HttpClient _client;

        private readonly string _apiKey;

        const string apiPath = "https://maps.googleapis.com/maps/api/geocode/json";
        public Geocoder(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient();
        }
        public async Task<string> ReverseGeocodeAsync(Coordinates coordinates)
        {
            var apiCall = _client.GetStreamAsync($"{apiPath}?latlng={coordinates.Longitude},{coordinates.Latitude}&language=lt&result_type=political&key={_apiKey}");
            var deserializedApiResponse = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(await apiCall);
            string formattedTitle = deserializedApiResponse.Results.First().FormattedAddress;
            return formattedTitle;
        }
    }
    internal class ReverseGeocodingApiResult
    {
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }
    }
    internal class ReverseGeocodingApiResponse
    {
        [JsonPropertyName("results")]
        public IEnumerable<ReverseGeocodingApiResult>? Results { get; set; }
    }
}

