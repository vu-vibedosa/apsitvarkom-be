using System.Text;
using System.Text.Json;
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
        var result = await GetAllAsync();
        return result.SingleOrDefault(loc => loc.Id == id); 
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationDTOFileRepository FromFile(string sourcePath)
    {
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

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}