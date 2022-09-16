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
    public PollutedLocationsDTOFileStore(string jsonString)
    {
        _jsonString = jsonString;
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public PollutedLocationDTO[] GetAllPollutedLocations()
    {
        var result = JsonSerializer.Deserialize<List<PollutedLocationDTO>>(_jsonString, _options);
        return result is not null ? result.ToArray() : Array.Empty<PollutedLocationDTO>();
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationsDTOFileStore FromFile(string sourcePath)
    {
        if (sourcePath is null)
            throw new ArgumentNullException(nameof(sourcePath), "Could not access file specified as the source path.");
        using var reader = new StreamReader(sourcePath);
        return FromContent(reader.ReadToEnd());
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationsDTOFileStore FromContent(string contents)
    {
        return new PollutedLocationsDTOFileStore(contents);
    }
}