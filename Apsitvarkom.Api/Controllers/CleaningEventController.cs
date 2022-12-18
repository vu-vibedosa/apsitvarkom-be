using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CleaningEventController : ControllerBase
{
    private readonly IRepository<CleaningEvent> _repository;
    private readonly IPollutedLocationRepository _pollutedLocationRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ObjectIdentifyRequest> _objectIdentifyValidator;
    private readonly IValidator<CleaningEventCreateRequest> _cleaningEventCreateValidator;
    private readonly IValidator<CleaningEventUpdateRequest> _cleaningEventUpdateValidator;
    private readonly IValidator<CleaningEventFinalizeRequest> _cleaningEventFinalizeValidator;

    public CleaningEventController(
        IRepository<CleaningEvent> repository,
        IPollutedLocationRepository pollutedLocationRepository,
        IMapper mapper, 
        IValidator<ObjectIdentifyRequest> objectIdentifyValidator,
        IValidator<CleaningEventCreateRequest> cleaningEventCreateValidator,
        IValidator<CleaningEventUpdateRequest> cleaningEventUpdateValidator,
        IValidator<CleaningEventFinalizeRequest> cleaningEventFinalizeValidator)
    {
        _repository = repository;
        _pollutedLocationRepository = pollutedLocationRepository;
        _mapper = mapper;
        _objectIdentifyValidator = objectIdentifyValidator;
        _cleaningEventCreateValidator = cleaningEventCreateValidator;
        _cleaningEventUpdateValidator = cleaningEventUpdateValidator;
        _cleaningEventFinalizeValidator = cleaningEventFinalizeValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<CleaningEventResponse>>> GetAll()
    {
        IEnumerable<CleaningEvent> events;
        try
        {
            events = await _repository.GetAllAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var sortedEvents = events.OrderBy(o => o.StartTime);

        var mappedEvents = _mapper.Map<IEnumerable<CleaningEventResponse>>(sortedEvents);
        if (mappedEvents is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedEvents);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<CleaningEventResponse>> GetById([FromQuery] ObjectIdentifyRequest cleaningEventIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(cleaningEventIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        CleaningEvent? cleaningEvent;
        try
        {
            cleaningEvent = await _repository.GetByPropertyAsync(x => x.Id == cleaningEventIdentifyRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (cleaningEvent is null) return NotFound($"Cleaning event with the specified id '{cleaningEventIdentifyRequest.Id}' was not found.");

        var mappedEvent = _mapper.Map<CleaningEventResponse>(cleaningEvent);
        if (mappedEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedEvent);
    }

    /// <param name="cleaningEventCreateRequest">The `Cleaning Event` to be created has to have a Start Time value in the future, as well as
    /// reference a `Polluted Location` whose progress value is less than 100 and which has no active `Cleaning Event`s.</param>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("Create")]
    public async Task<ActionResult<CleaningEventResponse>> Create(CleaningEventCreateRequest cleaningEventCreateRequest)
    {
        var validationResult = await _cleaningEventCreateValidator.ValidateAsync(cleaningEventCreateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        
        bool pollutedLocationExists;
        try
        {
            pollutedLocationExists = await _pollutedLocationRepository.ExistsByPropertyAsync(x => x.Id == cleaningEventCreateRequest.PollutedLocationId);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (pollutedLocationExists is false) return NotFound($"Polluted location with the specified id '{cleaningEventCreateRequest.PollutedLocationId}' was not found.");

        var cleaningEventDefaults = new CleaningEvent
        {
            Id = Guid.NewGuid()
        };

        var mappedCleaningEvent = _mapper.Map<CleaningEventCreateRequest, CleaningEvent>(cleaningEventCreateRequest, cleaningEventDefaults);
        if (mappedCleaningEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        var response = _mapper.Map<CleaningEventResponse>(mappedCleaningEvent);
        if (response is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.InsertAsync(mappedCleaningEvent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    /// <param name="cleaningEventUpdateRequest">The `Cleaning Event` to be updated has to be not yet finalized as well as have a Start Time value in the future.</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPut("Update")]
    public async Task<ActionResult<CleaningEventResponse>> Update(CleaningEventUpdateRequest cleaningEventUpdateRequest)
    {
        var validationResult = await _cleaningEventUpdateValidator.ValidateAsync(cleaningEventUpdateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        CleaningEvent? cleaningEvent;
        try
        {
            cleaningEvent = await _repository.GetByPropertyAsync(x => x.Id == cleaningEventUpdateRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (cleaningEvent is null) return NotFound($"Cleaning event with the specified id '{cleaningEventUpdateRequest.Id}' was not found.");

        var mappedEvent = _mapper.Map(cleaningEventUpdateRequest, cleaningEvent);
        if (mappedEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        var response = _mapper.Map<CleaningEventResponse>(mappedEvent);
        if (response is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.UpdateAsync(mappedEvent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(response);
    }

    /// <param name="cleaningEventFinalizeRequest">The `Cleaning Event` to be finalized has to be already finished but not yet finalized
    /// and the new progress value should be higher than the currently referenced `Polluted Location`'s progress value.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("Finalize")]
    public async Task<ActionResult> Finalize(CleaningEventFinalizeRequest cleaningEventFinalizeRequest)
    {
        var validationResult = await _cleaningEventFinalizeValidator.ValidateAsync(cleaningEventFinalizeRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        PollutedLocation? pollutedLocation;
        try
        {
            pollutedLocation = await _pollutedLocationRepository.GetByPropertyAsync(x => x.Events.Any(e => e.Id == cleaningEventFinalizeRequest.Id));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (pollutedLocation is null) return NotFound($"Cleaning event with the specified id '{cleaningEventFinalizeRequest.Id}' was not found.");

        var cleaningEvent = pollutedLocation.Events.Single(x => x.Id == cleaningEventFinalizeRequest.Id);

        pollutedLocation.Progress = (int)cleaningEventFinalizeRequest.NewProgress!;
        cleaningEvent.IsFinalized = true;

        try
        {
            await _pollutedLocationRepository.UpdateAsync(pollutedLocation);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpDelete("Delete")]
    public async Task<ActionResult> Delete([FromQuery] ObjectIdentifyRequest cleaningEventIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(cleaningEventIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        bool cleaningEventExists;
        try
        {
            cleaningEventExists = await _repository.ExistsByPropertyAsync(x => x.Id == cleaningEventIdentifyRequest.Id);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (cleaningEventExists is false) return NotFound($"Cleaning event with the specified id '{cleaningEventIdentifyRequest.Id}' was not found.");

        var mappedEvent = _mapper.Map<CleaningEvent>(cleaningEventIdentifyRequest);
        if (mappedEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        try
        {
            await _repository.DeleteAsync(mappedEvent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}