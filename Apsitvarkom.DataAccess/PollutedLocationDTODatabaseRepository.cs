using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Apsitvarkom.DataAccess;

public class PollutedLocationDTODatabaseRepository : ILocationDTORepository<PollutedLocationDTO>
{
    private readonly PollutedLocationContext _context;
    private readonly IMapper _mapper;

    public PollutedLocationDTODatabaseRepository(PollutedLocationContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync()
    {
        var models = await _context.PollutedLocations.ToListAsync();

        return _mapper.Map<List<PollutedLocation>, IEnumerable<PollutedLocationDTO>>(models);
    }

    public Task<IEnumerable<PollutedLocationDTO>> GetAllAsync(Location inRelationTo) => throw new NotImplementedException();

    public Task<PollutedLocationDTO?> GetByPropertyAsync(Func<PollutedLocationDTO, bool> propertyCondition) => throw new NotImplementedException();
}
