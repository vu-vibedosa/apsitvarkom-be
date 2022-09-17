using System.Text.Json;
using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationsDTOFileStore : IPollutedLocationDTORepository
{
    /// <summary>JSON string type file contents to be parsed.</summary>
    private readonly string _jsonString;

    /// <summary>Default options for objects deserialization.</summary>
    private readonly JsonSerializerOptions _options;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="jsonString">JSON string data to be used for parsing.</param>
    private PollutedLocationsDTOFileStore(string jsonString)
    {
        _jsonString = jsonString;
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public IEnumerable<PollutedLocationDTO> GetAllPollutedLocations()
    {
        var result = JsonSerializer.Deserialize<IEnumerable<PollutedLocationDTO>>(_jsonString, _options);
        return result ?? Enumerable.Empty<PollutedLocationDTO>();
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationsDTOFileStore FromFile(string sourcePath)
    {
        var contents = File.ReadAllText(sourcePath);
        return FromContent(contents);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationsDTOFileStore FromContent(string contents) => new(contents);
}