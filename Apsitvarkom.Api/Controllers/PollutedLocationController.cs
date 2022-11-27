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
    private readonly IGeocoder _geocoder;
    private readonly IValidator<CoordinatesCreateRequest> _coordinatesValidator;
    private readonly IValidator<PollutedLocationCreateRequest> _pollutedLocationValidator;
    private readonly IValidator<PollutedLocationUpdateRequest> _pollutedLocationUpdateValidator;

    public PollutedLocationController(
        IPollutedLocationRepository repository, 
        IMapper mapper,
        IGeocoder geocoder,
        IValidator<CoordinatesCreateRequest> coordinatesValidator, 
        IValidator<PollutedLocationCreateRequest> pollutedLocationValidator,
        IValidator<PollutedLocationUpdateRequest> pollutedLocationUpdateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _geocoder = geocoder;
        _coordinatesValidator = coordinatesValidator;
        _pollutedLocationValidator = pollutedLocationValidator;
        _pollutedLocationUpdateValidator = pollutedLocationUpdateValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationResponse>>> GetAll()
    {
        try
        {
            var locations = await _repository.GetAllAsync();

            var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationResponse>>(locations);
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
    public async Task<ActionResult<IEnumerable<PollutedLocationResponse>>> GetAll([FromQuery] CoordinatesCreateRequest coordinates)
    {
        var validationResult = await _coordinatesValidator.ValidateAsync(coordinates);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var mappedCoordinates = _mapper.Map<Coordinates>(coordinates);
        if (mappedCoordinates is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            var locations = await _repository.GetAllAsync(mappedCoordinates);

            var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationResponse>>(locations);
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
    public async Task<ActionResult<PollutedLocationResponse>> GetById(string id)
    {
        try
        {
            var location = await _repository.GetByPropertyAsync(x => x.Id.ToString() == id);

            if (location is null) return NotFound($"Polluted location with the specified id '{id}' was not found.");

            var mappedLocation = _mapper.Map<PollutedLocationResponse>(location);
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
    public async Task<ActionResult<PollutedLocationResponse>> Create(PollutedLocationCreateRequest pollutedLocationCreateRequest)
    {
        var validationResult = await _pollutedLocationValidator.ValidateAsync(pollutedLocationCreateRequest);
        if (!validationResult.IsValid) return BadRequest(error: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var mappedPollutedLocation = _mapper.Map<PollutedLocation>(pollutedLocationCreateRequest);
        if (mappedPollutedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        mappedPollutedLocation.Id = Guid.NewGuid();
        mappedPollutedLocation.Spotted = DateTime.UtcNow;
        mappedPollutedLocation.Location.Title = await _geocoder.ReverseGeocodeAsync(mappedPollutedLocation.Location.Coordinates) ?? string.Empty;

        var response = _mapper.Map<PollutedLocationResponse>(mappedPollutedLocation);
        if (response is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.InsertAsync(mappedPollutedLocation);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("Update")]
    public async Task<ActionResult<PollutedLocationResponse>> Update(PollutedLocationUpdateRequest pollutedLocationUpdateRequest)
    {
        var validatorResponse = await _pollutedLocationUpdateValidator.ValidateAsync(pollutedLocationUpdateRequest);
        if (!validatorResponse.IsValid) return BadRequest(validatorResponse.Errors.Select(e => e.ErrorMessage).ToList());

        var existingLocation = await _repository.GetByPropertyAsync(x => x.Id == pollutedLocationUpdateRequest.Id);
        if (existingLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);
       
        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(pollutedLocationUpdateRequest, existingLocation);
        if (mappedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.UpdateAsync(mappedLocation);

            return Ok(mappedLocation);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
