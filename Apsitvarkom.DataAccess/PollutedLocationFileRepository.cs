using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Mapping;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocation" /> data handling from file.
/// </summary>
public class PollutedLocationFileRepository : IPollutedLocationRepository, IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly Stream _stream;
    private readonly IMapper _mapper;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="stream">Stream to be used for parsing.</param>
    private PollutedLocationFileRepository(IMapper mapper, Stream stream)
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
    public async Task<IEnumerable<PollutedLocation>> GetAllAsync()
    {
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<PollutedLocationGetResponse>>(_stream, _options);

        return result switch
        {
            not null => _mapper.Map<IEnumerable<PollutedLocation>>(result),
            _ => Enumerable.Empty<PollutedLocation>()
        };
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocation>> GetAllAsync(Coordinates inRelationTo)
    {
        return from pollutedLocationDTO in await GetAllAsync()
               orderby _mapper.Map<PollutedLocation>(pollutedLocationDTO).Location.Coordinates.DistanceTo(inRelationTo)
               select pollutedLocationDTO;
    }

    /// <inheritdoc />
    public async Task<PollutedLocation?> GetByPropertyAsync(Expression<Func<PollutedLocation, bool>> propertyCondition)
    {
        var allLocations = await GetAllAsync();
        return allLocations.FirstOrDefault(propertyCondition.Compile());
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationFileRepository FromFile(IMapper mapper, string sourcePath)
    {
        var stream = File.OpenRead(sourcePath);
        return new PollutedLocationFileRepository(mapper, stream);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationFileRepository FromContent(IMapper mapper, string contents = "[]")
    {
        var byteArray = Encoding.UTF8.GetBytes(contents);
        var stream = new MemoryStream(byteArray);
        return new PollutedLocationFileRepository(mapper, stream);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}