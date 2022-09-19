using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Microsoft.AspNetCore.Mvc;
namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PollutedLocationController : ControllerBase
{
    private readonly ILogger<PollutedLocationController> _logger;

    public PollutedLocationController(ILogger<PollutedLocationController> logger)
    {
        _logger = logger;
    }


    private static IEnumerable<PollutedLocationDTO> pollutedLocations = new List<PollutedLocationDTO>
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
                Spotted  =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ttt"),
                Progress = 15,
                Notes = "This is test notes"

            }
        };

    [HttpGet("{id}")]
    public ActionResult<PollutedLocationDTO> GetById(string id)
    {
        var pollutedLocation = pollutedLocations.FirstOrDefault(PollutedPlace => PollutedPlace.Id == id);
        if (pollutedLocation == null)
        {
            return BadRequest("Polluted location is not found - wrong id provided or id is not provided at all");
        }
        return Ok(pollutedLocation);
    }

    [HttpGet]
    [Route("/[controller]/All")]
    public ActionResult<List<PollutedLocationDTO>> GetAll()
    {

        return Ok(pollutedLocations);
    }

    [HttpPost]
    public ActionResult<IEnumerable<PollutedLocationDTO>> Insert(PollutedLocationDTO PolutedLocation)
    {

        var list = pollutedLocations.ToList();
        list.Add(PolutedLocation);

        pollutedLocations = list;
        return Ok(PolutedLocation);
    }

    [HttpPut]
    public ActionResult<List<PollutedLocation>> Update(PollutedLocationDTO PollutedLocation)
    {

        var list = pollutedLocations.ToList();
        if (list.Remove(list.Where(predicate: x => x.Id == PollutedLocation.Id).FirstOrDefault()))
        {
            list.Add(PollutedLocation);
            pollutedLocations = list;
            return Ok(PollutedLocation);
        }
        return BadRequest();

    }

    [HttpDelete("{id}")]
    public ActionResult<PollutedLocationDTO> Delete(string id)
    {
        var foundPollutionLocation = pollutedLocations.FirstOrDefault(pollutedPlace => pollutedPlace.Id == id);
        if (foundPollutionLocation == null)
        {
            return BadRequest("Heroe not found");
        }
        var pollutedLocationList = pollutedLocations.ToList();
        pollutedLocationList.Remove(foundPollutionLocation);
        pollutedLocations = pollutedLocationList;
        return Ok(pollutedLocations);
    }
}
