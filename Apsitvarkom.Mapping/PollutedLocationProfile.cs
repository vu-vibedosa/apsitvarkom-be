using Apsitvarkom.DataAccess;
using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.Mapping;

/// <summary>
/// Implements a profile for AutoMapper that allows creating maps required for conversion of class
/// <see cref="PollutedLocation" /> related objects (Requests, Responses, Business objects).
/// </summary>
public class PollutedLocationProfile : Profile
{
    public PollutedLocationProfile()
    {
        MapRequests();
        MapResponses();
    }

    private void MapRequests()
    {
        CreateMap<CoordinatesCreateRequest, Coordinates>();
        CreateMap<LocationCreateRequest, Location>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom<LocationTitleResolver>());
        CreateMap<PollutedLocationCreateRequest, PollutedLocation>()
            .ForMember(dest => dest.Spotted, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }

    private void MapResponses()
    {
        CreateMap<PollutedLocation, PollutedLocationResponse>();
        CreateMap<Location, LocationResponse>();
        CreateMap<Coordinates, CoordinatesResponse>();
    }
}

public class LocationTitleResolver : IValueResolver<LocationCreateRequest, Location, string>
{
    private readonly IGeocoder _geocoder;

    public LocationTitleResolver(IGeocoder geocoder)
    {
        _geocoder = geocoder;
    }

    public string Resolve(LocationCreateRequest source, Location destination, string destMember, ResolutionContext context)
    {
        return Task.Run(async () => await _geocoder.ReverseGeocodeAsync(destination.Coordinates)).Result ?? string.Empty;
    }
}