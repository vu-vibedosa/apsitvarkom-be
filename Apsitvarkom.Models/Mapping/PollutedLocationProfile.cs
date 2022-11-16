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
        CreateMap<CoordinatesCreateRequest, Location>()
            .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(source => source));
        CreateMap<PollutedLocationCreateRequest, PollutedLocation>()
            .ForMember(dest => dest.Spotted, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(source => source.Coordinates));
    }

    private void MapResponses()
    {
        CreateMap<PollutedLocation, PollutedLocationGetResponse>();
        CreateMap<Location, LocationGetResponse>();
        CreateMap<Coordinates, CoordinatesGetResponse>();
    }
}