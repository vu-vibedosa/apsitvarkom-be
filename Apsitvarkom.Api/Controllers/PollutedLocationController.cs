using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PollutedLocationController : ControllerBase
{
    private readonly IPollutedLocationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CoordinatesGetRequest> _coordinatesValidator;

    public PollutedLocationController(IPollutedLocationRepository repository, IMapper mapper, IValidator<CoordinatesGetRequest> coordinatesValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _coordinatesValidator = coordinatesValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationGetResponse>>> GetAll()
    {
        try
        {
            var locations = await _repository.GetAllAsync();
            return Ok(locations);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ValidationFailure>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All/OrderedByDistance")]
    public async Task<ActionResult<IEnumerable<PollutedLocationGetResponse>>> GetAll([FromQuery] CoordinatesGetRequest coordinates)
    {
        var validationResult = await _coordinatesValidator.ValidateAsync(coordinates);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var mappedCoordinates = _mapper.Map<Coordinates>(coordinates);
        if (mappedCoordinates is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            var locations = await _repository.GetAllAsync(mappedCoordinates);
            return Ok(locations);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PollutedLocationGetResponse>> GetById(string id)
    {
        try
        {
            var location = await _repository.GetByPropertyAsync(x => x.Id.ToString() == id);
            return location is not null ? Ok(location) : NotFound($"Polluted location with the specified id '{id}' was not found.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}