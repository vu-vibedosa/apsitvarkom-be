using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="T" /> class data handling, where <see cref="T"/> derives from <see cref="LocationDTO"/>.
/// </summary>
public interface ILocationDTORepository<T> : IDTORepository<T> where T : LocationDTO
{
    /// <summary>Gets all <see cref="T" /> records from the data source sorted in ascending order by distance from the given location.</summary>
    /// <param name="inRelationTo">Starting <see cref="Location"/> point for distance measurement.</param>
    /// <returns><see cref="T" /> of ordered <see cref="T" /> instances by distance.</returns>
    public Task<IEnumerable<T>> GetAllAsync(Location inRelationTo);
}