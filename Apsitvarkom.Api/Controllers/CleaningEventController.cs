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
    private readonly IRepository<CleaningEvent> _cleaningEventRepository;
    private readonly IPollutedLocationRepository _pollutedLocationRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ObjectIdentifyRequest> _objectIdentifyValidator;
    private readonly IValidator<CleaningEventUpdateRequest> _cleaningEventUpdateValidator;

    public CleaningEventController(
        IRepository<CleaningEvent> cleaningEventRepository,
        IPollutedLocationRepository pollutedLocationRepository,
        IMapper mapper, 
        IValidator<ObjectIdentifyRequest> objectIdentifyValidator,
        IValidator<CleaningEventUpdateRequest> cleaningEventUpdateValidator)
    {
        _cleaningEventRepository = cleaningEventRepository;
        _pollutedLocationRepository = pollutedLocationRepository;
        _mapper = mapper;
        _objectIdentifyValidator = objectIdentifyValidator;
        _cleaningEventUpdateValidator = cleaningEventUpdateValidator;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<CleaningEventResponse>>> GetAll()
    {
        IEnumerable<CleaningEvent> events;
        try
        {
            events = await _cleaningEventRepository.GetAllAsync();
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
            cleaningEvent = await _cleaningEventRepository.GetByPropertyAsync(x => x.Id == cleaningEventIdentifyRequest.Id);
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("Update")]
    public async Task<ActionResult<CleaningEventResponse>> Update(CleaningEventUpdateRequest cleaningEventUpdateRequest)
    {
        var validationResult = await _cleaningEventUpdateValidator.ValidateAsync(cleaningEventUpdateRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        CleaningEvent? cleaningEvent;
        try
        {
            cleaningEvent = await _cleaningEventRepository.GetByPropertyAsync(x => x.Id == cleaningEventUpdateRequest.Id);
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
            await _cleaningEventRepository.UpdateAsync(mappedEvent);
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
    public async Task<ActionResult> Delete([FromQuery] ObjectIdentifyRequest cleaningEventIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(cleaningEventIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        bool cleaningEventExists;
        try
        {
            cleaningEventExists = await _cleaningEventRepository.ExistsByPropertyAsync(x => x.Id == cleaningEventIdentifyRequest.Id);
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
            await _cleaningEventRepository.DeleteAsync(mappedEvent);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}