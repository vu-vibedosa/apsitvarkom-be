﻿using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Apsitvarkom.Models;
using Apsitvarkom.Models.DTO;
using Apsitvarkom.Models.Mapping;
using AutoMapper;

namespace Apsitvarkom.DataAccess;

/// <summary>
/// Class for <see cref="PollutedLocationDTO" /> data handling from file.
/// </summary>
public class PollutedLocationDTOFileRepository : IPollutedLocationDTORepository, IDisposable
{
    private readonly JsonSerializerOptions _options;
    private readonly Stream _stream;
    private readonly IMapper _mapper;

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
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<PollutedLocationProfile>()).CreateMapper();
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
        return from pollutedLocation in await GetAllAsync()
               where pollutedLocation != null
               orderby inRelationTo.DistanceTo(_mapper.Map<Location>(pollutedLocation.Location))
               select pollutedLocation;
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
        var stream = File.OpenRead(CheckIfFileIsJson(sourcePath));
        return new PollutedLocationDTOFileRepository(stream);
    }

    /// <summary>Static factory constructor for reader from JSON string.</summary>
    /// <param name="contents">Contents to be parsed from JSON string.</param>
    public static PollutedLocationDTOFileRepository FromContent(string contents = "[]")
    {
        var byteArray = Encoding.UTF8.GetBytes(contents);
        var stream = new MemoryStream(byteArray);
        return new PollutedLocationDTOFileRepository(stream);
    }
    
    /// <summary>Checks if the file path points to a file with .json extension.</summary>
    /// <param name="path">Path being checked.</param>
    /// <returns>Path on success, <see cref="FormatException"/> otherwise.</returns>
    private static string CheckIfFileIsJson(string path)
    {   
        const string regexPattern = @"(\.json)$";
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