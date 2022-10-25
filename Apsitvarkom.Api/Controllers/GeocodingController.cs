using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GeocodingController
    {
        private readonly IGeocoder _geocoder;

        public GeocodingController(IGeocoder geocoder)
        {
            _geocoder = geocoder;
        }
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<string> getGeocoder()
        {
            var coordinates = new Coordinates
            {
                Longitude = 54.687203,
                Latitude = 25.241313,
            };
            string response = await _geocoder.ReverseGeocode(coordinates);
            return response;
        }
    }
}
