using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Interface for <see cref="PollutedLocationDTO" /> class data handling.
/// </summary>
public interface IPollutedLocationDTORepository
{
    /// <summary>Gets all <see cref="PollutedLocationDTO" /> records from the data source.</summary>
    /// <returns>Array of all <see cref="PollutedLocationDTO" /> instances.</returns>
    public PollutedLocationDTO[] GetAllPollutedLocations();
}