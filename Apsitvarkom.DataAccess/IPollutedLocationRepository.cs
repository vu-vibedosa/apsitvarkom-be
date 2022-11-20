using Apsitvarkom.Models;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="PollutedLocation" /> class data handling.
/// </summary>
public interface IPollutedLocationRepository : IRepository<PollutedLocation>
{
    /// <summary>Gets all <see cref="PollutedLocation" /> records from the data source sorted in ascending order by distance from the given location.</summary>
    /// <param name="inRelationTo">Starting <see cref="Coordinates"/> point for distance measurement.</param>
    /// <returns><see cref="PollutedLocation" /> of ordered <see cref="PollutedLocation" /> instances by distance.</returns>
    public Task<IEnumerable<PollutedLocation>> GetAllAsync(Coordinates inRelationTo);
}