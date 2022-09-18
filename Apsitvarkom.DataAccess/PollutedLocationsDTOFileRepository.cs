using System.Text;
using System.Text.Json;
using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationsDTOFileRepository : IPollutedLocationDTORepository, IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly Stream _stream;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="stream">Stream to be used for parsing.</param>
    private PollutedLocationsDTOFileRepository(Stream stream)
    {
        _stream = stream;
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllPollutedLocationsAsync()
    {
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<PollutedLocationDTO>>(_stream, _options);
        return result ?? Enumerable.Empty<PollutedLocationDTO>();
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationsDTOFileRepository FromFile(string sourcePath)
    {
        var stream = File.OpenRead(sourcePath);
        return new PollutedLocationsDTOFileRepository(stream);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationsDTOFileRepository FromContent(string contents)
    {
        var byteArray = Encoding.UTF8.GetBytes(contents);
        var stream = new MemoryStream(byteArray);
        return new PollutedLocationsDTOFileRepository(stream);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}