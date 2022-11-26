using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Apsitvarkom.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TidyingEventController : ControllerBase
{
    private readonly IRepository<TidyingEvent> _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<ObjectIdentifyRequest> _objectIdentifyValidator;

    public TidyingEventController(
        IRepository<TidyingEvent> repository, 
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
    public async Task<ActionResult<IEnumerable<TidyingEventResponse>>> GetAll()
    {
        IEnumerable<TidyingEvent> events;
        try
        {
            events = await _repository.GetAllAsync();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var mappedEvents = _mapper.Map<IEnumerable<TidyingEventResponse>>(events);
        if (mappedEvents is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedEvents);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet]
    public async Task<ActionResult<TidyingEventResponse>> GetById([FromQuery] ObjectIdentifyRequest tidyingEventIdentifyRequest)
    {
        var validationResult = await _objectIdentifyValidator.ValidateAsync(tidyingEventIdentifyRequest);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        TidyingEvent? tidyingEvent;
        try
        {
            tidyingEvent = await _repository.GetByPropertyAsync(x => x.Id == tidyingEventIdentifyRequest.Id);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (tidyingEvent is null) return NotFound($"Polluted location with the specified id '{tidyingEventIdentifyRequest.Id}' was not found.");

        var mappedEvent = _mapper.Map<TidyingEventResponse>(tidyingEvent);
        if (mappedEvent is null) return StatusCode(StatusCodes.Status500InternalServerError);

        return Ok(mappedEvent);
    }
}