using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="PollutedLocationDTO" /> class data handling.
/// </summary>
public interface IPollutedLocationDTORepository
{
    /// <summary>Gets all <see cref="PollutedLocationDTO" /> records from the data source.</summary>
    /// <returns><see cref="Enumerable" /> of <see cref="PollutedLocationDTO" /> instances.</returns>
    public Task<IEnumerable<PollutedLocationDTO>> GetAllAsync();

    /// <summary>Gets all <see cref="PollutedLocationDTO" /> records from the data source sorted in ascending order by distance from the given location.</summary>
    /// <param name="inRelationTo">Starting <see cref="Location"/> point for distance measurement.</param>
    /// <returns><see cref="Enumerable" /> of ordered <see cref="PollutedLocationDTO" /> instances by distance.</returns>
    public Task<IEnumerable<PollutedLocationDTO>> GetAllAsync(Location inRelationTo);

    /// <summary>Gets a single <see cref="PollutedLocationDTO" /> record from the data source by id.</summary>
    /// <param name="id">Identifier of the requested record.</param>
    /// <returns>Null if the instance was not found. Otherwise a <see cref="PollutedLocationDTO" /> instance with matching identifier.</returns>
    public Task<PollutedLocationDTO?> GetByIdAsync(string id);
}