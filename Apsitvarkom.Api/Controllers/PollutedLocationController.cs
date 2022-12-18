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
    private readonly IValidator<PollutedLocationCreateRequest> _pollutedLocationCreateValidator;
    private readonly IValidator<ObjectIdentifyRequest> _objectIdentifyValidator;
    private readonly IValidator<PollutedLocationUpdateRequest> _pollutedLocationUpdateValidator;

    public PollutedLocationController(
        IPollutedLocationRepository repository,
        IMapper mapper,
        IGeocoder geocoder,
        IValidator<CoordinatesCreateRequest> coordinatesValidator, 
        IValidator<PollutedLocationCreateRequest> pollutedLocationCreateValidator,
        IValidator<ObjectIdentifyRequest> objectIdentifyValidator,
        IValidator<PollutedLocationUpdateRequest> pollutedLocationUpdateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _geocoder = geocoder;
        _coordinatesValidator = coordinatesValidator;
        _pollutedLocationCreateValidator = pollutedLocationCreateValidator;
        _objectIdentifyValidator = objectIdentifyValidator;
        _pollutedLocationUpdateValidator = pollutedLocationUpdateValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationResponse>>> GetAll()
    {
        IEnumerable<PollutedLocation> locations;
        try
        {
            locations = await _repository.GetAllAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var sortedLocations = locations.OrderBy(o => o.Spotted);

        var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationResponse>>(sortedLocations);
        if (mappedLocations is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedLocations);
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

        IEnumerable<PollutedLocation> locations;
        try
        {
            locations = await _repository.GetAllAsync(mappedCoordinates);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var mappedLocations = _mapper.Map<IEnumerable<PollutedLocationResponse>>(locations);
        if (mappedLocations is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedLocations);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<PollutedLocationResponse>> GetById([FromQuery] ObjectIdentifyRequest pollutedLocationIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(pollutedLocationIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        PollutedLocation? location;
        try
        {
            location = await _repository.GetByPropertyAsync(x => x.Id == pollutedLocationIdentifyRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (location is null) return NotFound($"Polluted location with the specified id '{pollutedLocationIdentifyRequest.Id}' was not found.");

        var mappedLocation = _mapper.Map<PollutedLocationResponse>(location);
        if (mappedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedLocation);
    }

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Create")]
    public async Task<ActionResult<PollutedLocationResponse>> Create(PollutedLocationCreateRequest pollutedLocationCreateRequest)
    {
        var validationResult = await _pollutedLocationCreateValidator.ValidateAsync(pollutedLocationCreateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        var mappedLocation = _mapper.Map<Location>(pollutedLocationCreateRequest.Location);
        if (mappedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        var title = await _geocoder.ReverseGeocodeAsync(mappedLocation.Coordinates);

        var pollutedLocationDefaults = new PollutedLocation
        {
            Id = Guid.NewGuid(),
            Spotted = DateTime.UtcNow,
            Progress = 0,
            Location = 
            {
                Title = title
            }
        };

        var mappedPollutedLocation = _mapper.Map<PollutedLocationCreateRequest, PollutedLocation>(pollutedLocationCreateRequest, pollutedLocationDefaults);
        if (mappedPollutedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

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

    /// <param name="pollutedLocationUpdateRequest">The progress of the `Polluted Location` to be updated has to be not equal to 100.</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("Update")]
    public async Task<ActionResult<PollutedLocationResponse>> Update(PollutedLocationUpdateRequest pollutedLocationUpdateRequest)
    {
        var validationResult = await _pollutedLocationUpdateValidator.ValidateAsync(pollutedLocationUpdateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        PollutedLocation? location;
        try
        {
            location = await _repository.GetByPropertyAsync(x => x.Id == pollutedLocationUpdateRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (location is null) return NotFound($"Polluted location with the specified id '{pollutedLocationUpdateRequest.Id}' was not found.");

        var mappedLocation = _mapper.Map<PollutedLocationUpdateRequest, PollutedLocation>(pollutedLocationUpdateRequest, location);
        if (mappedLocation is null) return StatusCode(StatusCodes.Status500InternalServerError);

        var response = _mapper.Map<PollutedLocationResponse>(mappedLocation);
        if (response is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.UpdateAsync(mappedLocation);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("Delete")]
    public async Task<ActionResult> Delete([FromQuery] ObjectIdentifyRequest pollutedLocationIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(pollutedLocationIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        bool locationExists;
        try
        {
            locationExists = await _repository.ExistsByPropertyAsync(x => x.Id == pollutedLocationIdentifyRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        if (locationExists is false) return NotFound($"Polluted location with the specified id '{pollutedLocationIdentifyRequest.Id}' was not found.");

        var location = _mapper.Map<PollutedLocation>(pollutedLocationIdentifyRequest);
        if (location is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.DeleteAsync(location);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}