using Apsitvarkom.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace Apsitvarkom.DataAccess
{
    public class Geocoder : IGeocoder
    {
        private static readonly HttpClient client = new HttpClient();

        public static string geocoderApiKey;

        public async Task<string> ReverseGeocode(Coordinates cords)
        {
            var apiResponse = await MakeReverseGeocodeApiCall(cords);
            dynamic dynamicObject = JsonConvert.DeserializeObject<dynamic>(apiResponse)!;
            string locationTitle = dynamicObject.results[0].formatted_address;
            return locationTitle;
        }

        private static async Task<string> MakeReverseGeocodeApiCall(Coordinates cordinates)
        {
            var apiCall = client.GetStringAsync("https://maps.googleapis.com/maps/api/geocode/json?latlng=54.687203,25.241313&language=lt&result_type=political&key=[ENTER_API_KEY_THERE]");
            var apiResponse = await apiCall;
            return apiResponse;
        }

    }





}
