using Apsitvarkom.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PollutedLocationController : ControllerBase
{
    [HttpGet("All")]
    public ActionResult<IEnumerable<PollutedLocationDTO>> GetAll()
    {
        IEnumerable<PollutedLocationDTO> testData = new List<PollutedLocationDTO>()
        {
            new PollutedLocationDTO
            {
                Id = "5be2354e-2500-4289-bbe2-66210592e17f",
                Location = new LocationDTO
                {
                    Latitude = 12.1651,
                    Longitude = 121.151,
                },
                Radius = 10,
                Severity = "MODERATE",
                Spotted  =  DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                Progress = 15,
                Notes = "This is test notes Nr.1"
            },
            new PollutedLocationDTO
            {
                Id = "5be2354e-2500-4289-bbe2-66210592e17z",
                Location = new LocationDTO
                {
                    Latitude = 15.1651,
                    Longitude = 177.151,
                },
                Radius = 100,
                Severity = "MODERATE",
                Spotted  =  DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
                Progress = 55,
                Notes = "This is test notes Nr.2"
            }
        };
        return Ok(testData);
    }
}
