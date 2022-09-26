﻿using Apsitvarkom.DataAccess;
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("All")]
    public async Task<ActionResult<IEnumerable<PollutedLocationDTO>>> GetAll()
    {
        var instances = await _repository.GetAllAsync();
        return Ok(instances);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PollutedLocationDTO>> GetById(string id)
    {
        var instance = await _repository.GetByIdAsync(id);
        return instance is not null ? Ok(instance) : NotFound($"PollutedLocation with the specified id '{id}' was not found.");
    }
}