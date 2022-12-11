using Apsitvarkom.Models;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.ModelActions.Mapping;

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
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Progress, opt => opt.Ignore())
            .ForMember(x => x.Events, opt => opt.Ignore());
        CreateMap<ObjectIdentifyRequest, PollutedLocation>(MemberList.None)
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));
        CreateMap<PollutedLocationUpdateRequest, PollutedLocation>(MemberList.None)
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Radius, opt => opt.MapFrom(x => x.Radius))
            .ForMember(x => x.Severity, opt => opt.MapFrom(x => x.Severity))
            .ForMember(x => x.Notes, opt => opt.MapFrom(x => x.Notes));
    }

    private void MapResponses()
    {
        CreateMap<PollutedLocation, PollutedLocationResponse>();
        CreateMap<Location, LocationResponse>();
        CreateMap<Coordinates, CoordinatesResponse>();
        CreateMap<Translated<string>, TranslatedResponse<string>>()
            .ForMember(x => x.Lt, opt => opt.MapFrom(x => x.Lithuanian))
            .ForMember(x => x.En, opt => opt.MapFrom(x => x.English));
    }
}