using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Apsitvarkom.Models.DTO;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationDTOFileRepository : IPollutedLocationDTORepository, IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly Stream _stream;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="stream">Stream to be used for parsing.</param>
    private PollutedLocationDTOFileRepository(Stream stream)
    {
        _stream = stream;
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync()
    {
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<PollutedLocationDTO>>(_stream, _options);
        return result ?? Enumerable.Empty<PollutedLocationDTO>();
    }

    /// <inheritdoc />
    public async Task<PollutedLocationDTO?> GetByIdAsync(string id)
    {
        var allLocations = await GetAllAsync();
        return allLocations.SingleOrDefault(loc => loc.Id == id); 
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationDTOFileRepository FromFile(string sourcePath)
    {
        CheckIfFileIsJson(sourcePath);
        var stream = File.OpenRead(sourcePath);
        return new PollutedLocationDTOFileRepository(stream);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationDTOFileRepository FromContent(string contents)
    {
        var byteArray = Encoding.UTF8.GetBytes(contents);
        var stream = new MemoryStream(byteArray);
        return new PollutedLocationDTOFileRepository(stream);
    }
    
    /// <summary> Checks if file path ending is .json. </summary>
    /// <param name="path">Path which is being cheked.</param>
    private static void CheckIfFileIsJson(string path)
    {
        var regex = @"/*(\.json)";
        var match = Regex.Match(path, regex);
        if (!match.Success)
            throw new FormatException("File extension is not .json!");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}