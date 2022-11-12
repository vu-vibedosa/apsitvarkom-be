using System.Text;
using System.Text.Json;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;
using Apsitvarkom.Utilities;
using Microsoft.Extensions.Logging;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationDTOFileRepository : ILocationDTORepository<PollutedLocationDTO>, IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly Stream _stream;
    private readonly IMapper _mapper;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="stream">Stream to be used for parsing.</param>
    private PollutedLocationDTOFileRepository(IMapper mapper, Stream stream)
    {
        _stream = stream;
        _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync()
    {
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<PollutedLocationDTO>>(_stream, _options);
        return result ?? Enumerable.Empty<PollutedLocationDTO>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync(Location inRelationTo)
    {
        return from pollutedLocationDTO in await GetAllAsync()
               orderby inRelationTo.DistanceTo(_mapper.Map<PollutedLocation>(pollutedLocationDTO))
               select pollutedLocationDTO;
    }

    /// <inheritdoc />
    public async Task<PollutedLocationDTO?> GetByPropertyAsync(Func<PollutedLocationDTO, bool> propertyCondition)
    {
        var allLocations = await GetAllAsync();
        return allLocations.FirstOrDefault(propertyCondition);
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationDTOFileRepository FromFile(IMapper mapper, string sourcePath)
    {
        var stream = File.OpenRead(sourcePath);
        return new PollutedLocationDTOFileRepository(mapper, stream);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationDTOFileRepository FromContent(IMapper mapper, string contents = "[]")
    {
        var byteArray = Encoding.UTF8.GetBytes(contents);
        var stream = new MemoryStream(byteArray);
        return new PollutedLocationDTOFileRepository(mapper, stream);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}