using Apsitvarkom.DataAccess;
using Apsitvarkom.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PollutedLocationController : ControllerBase
{
    private readonly IPollutedLocationDTORepository _repository;

    public PollutedLocationController(IPollutedLocationDTORepository repository)
    {
        _repository = repository;
    }

    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationDTO>>> GetAll()
    {
        var instances = await _repository.GetAllAsync();
        return Ok(instances);
    }
}