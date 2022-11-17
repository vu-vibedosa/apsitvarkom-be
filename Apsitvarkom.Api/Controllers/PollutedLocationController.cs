using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PollutedLocationController : ControllerBase
{
    private readonly IPollutedLocationRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CoordinatesCreateRequest> _coordinatesValidator;
    private readonly IValidator<PollutedLocationCreateRequest> _pollutedLocationValidator;

    public PollutedLocationController(
        IPollutedLocationRepository repository, 
        IMapper mapper, 
        IValidator<CoordinatesCreateRequest> coordinatesValidator, 
        IValidator<PollutedLocationCreateRequest> pollutedLocationValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _coordinatesValidator = coordinatesValidator;
        _pollutedLocationValidator = pollutedLocationValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationGetResponse>>> GetAll()
    {
        try
        {
            var locations = await _repository.GetAllAsync();

            var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationGetResponse>>(locations);
            if (mappedLocations is null) return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(mappedLocations);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All/OrderedByDistance")]
    public async Task<ActionResult<IEnumerable<PollutedLocationGetResponse>>> GetAll([FromQuery] CoordinatesCreateRequest coordinates)
    {
        var validationResult = await _coordinatesValidator.ValidateAsync(coordinates);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var mappedCoordinates = _mapper.Map<Coordinates>(coordinates);
        if (mappedCoordinates is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            var locations = await _repository.GetAllAsync(mappedCoordinates);

            var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationGetResponse>>(locations);
            if (mappedLocations is null) return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(mappedLocations);
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

            if (location is null) return NotFound($"Polluted location with the specified id '{id}' was not found.");

            var mappedLocation = _mapper.Map<PollutedLocationGetResponse>(location);
            if (mappedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(mappedLocation);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Create")]
    public async Task<ActionResult<PollutedLocationGetResponse>> Create(PollutedLocationCreateRequest pollutedLocationCreateRequest)
    {
        var validationResult = await _pollutedLocationValidator.ValidateAsync(pollutedLocationCreateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var mappedPollutedLocation = _mapper.Map<PollutedLocation>(pollutedLocationCreateRequest);
        if (mappedPollutedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            var result = await _repository.InsertAsync(mappedPollutedLocation);

            var response = _mapper.Map<PollutedLocationGetResponse>(result);
            // If failed to map the response, still return 201 because the location was inserted into the database
            if (response is null) return StatusCode(StatusCodes.Status201Created);

            return CreatedAtAction(nameof(GetById), new { response.Id }, response);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}