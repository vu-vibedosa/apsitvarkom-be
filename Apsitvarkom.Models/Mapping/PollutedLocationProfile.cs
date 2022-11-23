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
        CreateMap<LocationCreateRequest, Location>()
            .ForMember(x => x.Title, opt => opt.Ignore());
        CreateMap<PollutedLocationCreateRequest, PollutedLocation>()
            .ForMember(x => x.Spotted, opt => opt.Ignore())
            .ForMember(x => x.Id, opt => opt.Ignore());
        CreateMap<PollutedLocationUpdateRequest, PollutedLocation>();
    }

    private void MapResponses()
    {
        CreateMap<PollutedLocation, PollutedLocationResponse>();
        CreateMap<Location, LocationResponse>();
        CreateMap<Coordinates, CoordinatesResponse>();
    }
}