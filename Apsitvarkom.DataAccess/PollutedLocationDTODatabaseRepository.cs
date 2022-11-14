﻿using System.Linq.Expressions;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling using database.
/// </summary>
public class PollutedLocationDTODatabaseRepository : ILocationDTORepository<PollutedLocationDTO>
{
    private readonly IPollutedLocationContext _context;
    private readonly IMapper _mapper;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="context">Database context for <see cref="PollutedLocation"/>.</param>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    public PollutedLocationDTODatabaseRepository(IPollutedLocationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync()
    {
        var models = await _context.PollutedLocations.ToListAsync();
        return _mapper.Map<List<PollutedLocation>, IEnumerable<PollutedLocationDTO>>(models);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync(Location inRelationTo)
    {
        return from pollutedLocation in await _context.PollutedLocations.ToListAsync()
            orderby inRelationTo.DistanceTo(pollutedLocation)
            select _mapper.Map<PollutedLocation, PollutedLocationDTO>(pollutedLocation);
    }

    /// <inheritdoc />
    public async Task<PollutedLocationDTO?> GetByPropertyAsync(Expression<Func<PollutedLocationDTO, bool>> propertyCondition)
    {
        return await _context.PollutedLocations
            .ProjectTo<PollutedLocationDTO>(_mapper.ConfigurationProvider)
            .Where(propertyCondition)
            .FirstOrDefaultAsync();
    }
}
