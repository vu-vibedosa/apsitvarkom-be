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
    private readonly IMapper _mapper;
    private readonly IValidator<ObjectIdentifyRequest> _objectIdentifyValidator;

    public CleaningEventController(
        IRepository<CleaningEvent> repository, 
        IMapper mapper, 
        IValidator<ObjectIdentifyRequest> objectIdentifyValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _objectIdentifyValidator = objectIdentifyValidator;
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

        var mappedEvents = _mapper.Map<IEnumerable<CleaningEventResponse>>(events);
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

        if (cleaningEvent is null) return NotFound($"Polluted location with the specified id '{cleaningEventIdentifyRequest.Id}' was not found.");

        var mappedEvent = _mapper.Map<CleaningEventResponse>(cleaningEvent);
        if (mappedEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedEvent);
    }
}