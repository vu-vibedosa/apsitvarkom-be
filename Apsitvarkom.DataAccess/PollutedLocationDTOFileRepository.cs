using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationDTOFileRepository : ILocationDTORepository<PollutedLocationDTO>, IDisposable
{
    private readonly JsonSerializerSettings _options;
    private readonly Stream _stream;
    private readonly IMapper _mapper;

    /// <summary>Constructor for the reader.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="stream">Stream to be used for parsing.</param>
    private PollutedLocationDTOFileRepository(IMapper mapper, Stream stream)
    {
        _stream = stream;
        _options = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
        _mapper = mapper;
    }

    /// <inheritdoc />
    public Task<IEnumerable<PollutedLocationDTO>> GetAllAsync()
    {
        return Task.Run(() =>
        {
            var reader = new StreamReader(_stream);
            var jsonString = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<IEnumerable<PollutedLocationDTO>>(jsonString, _options);
            return result ?? Enumerable.Empty<PollutedLocationDTO>();
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PollutedLocationDTO>> GetAllAsync(Location inRelationTo)
    {
        return from pollutedLocationDTO in await GetAllAsync()
               orderby inRelationTo.DistanceTo(_mapper.Map<PollutedLocation>(pollutedLocationDTO))
               select pollutedLocationDTO;
    }

    /// <inheritdoc />
    public async Task<PollutedLocationDTO?> GetByIdAsync(string id)
    {
        var allLocations = await GetAllAsync();
        return allLocations.SingleOrDefault(loc => loc.Id == id);
    }

    /// <summary>Static factory constructor for reader from file.</summary>
    /// <param name="mapper">Mapper implementing profile <see cref="PollutedLocationProfile"/>.</param>
    /// <param name="sourcePath">Relative location of the .json type source data file.</param>
    public static PollutedLocationDTOFileRepository FromFile(IMapper mapper, string sourcePath)
    {
        var stream = File.OpenRead(ValidateIfFilePathIsValid(sourcePath));
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

    /// <summary>Checks if the file path points to a file with .json extension.</summary>
    /// <param name="path">Path being checked.</param>
    /// <returns>Path on success, <see cref="FormatException"/> otherwise.</returns>
    private static string ValidateIfFilePathIsValid(string path)
    {   
        const string regexPattern = @"^[\w `~!@#$%^&()_+\-=\]}\/\\:[{;',.]+(\.json)$";
        var match = Regex.Match(path, regexPattern);
        if (!match.Success)
            throw new FormatException("File extension is not .json!");
        return path;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
        GC.SuppressFinalize(this);
    }
}