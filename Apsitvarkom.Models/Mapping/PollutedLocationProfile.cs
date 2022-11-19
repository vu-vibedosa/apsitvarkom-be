using System.Globalization;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.Models.Mapping;

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
        CreateMap<LocationCreateRequest, Location>();
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