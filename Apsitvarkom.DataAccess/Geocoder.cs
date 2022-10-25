using Apsitvarkom.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Apsitvarkom.DataAccess
{
    public class Geocoder : IGeocoder
    {
        private static HttpClient _client;

        private static string? _apiKey;
        public Geocoder(string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient();
        }
        public async Task<string> ReverseGeocode(Coordinates coordinates)
        {
            var apiCall = _client.GetStreamAsync("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + coordinates.Longitude + "," + coordinates.Latitude + "&language=lt&result_type=political&key=" + _apiKey);
            var deserializedApiResponse = await JsonSerializer.DeserializeAsync<ReverseGeocodingApiResponse>(await apiCall);
            string formattedTitle = deserializedApiResponse.results.First().formatted_address;
            return formattedTitle;
        }
    }
    public class ReverseGeocodingApiResult
    {
        public string? formatted_address { get; set; }
    }
    public class ReverseGeocodingApiResponse
    {
        public IEnumerable<ReverseGeocodingApiResult>? results { get; set; }
    }
}
