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
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Progress, opt => opt.Ignore())
            .ForMember(x => x.Events, opt => opt.Ignore());
        CreateMap<ObjectIdentifyRequest, PollutedLocation>(MemberList.None)
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));
        CreateMap<PollutedLocationUpdateRequest, PollutedLocation>()
            .ForMember(x => x.Spotted, opt => opt.Ignore())
            .ForMember(x => x.Location, opt => opt.Ignore())
            .ForMember(x => x.Events, opt => opt.Ignore())
            .ForMember(x => x.Progress, opt => opt.PreCondition(src => src.Progress is not null))
            .ForMember(x => x.Radius, opt => opt.PreCondition(src => src.Radius is not null))
            .ForMember(x => x.Severity, opt => opt.PreCondition(src => src.Severity is not null))
            .ForMember(x => x.Notes, opt => opt.PreCondition(src => src.Notes is not null));
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