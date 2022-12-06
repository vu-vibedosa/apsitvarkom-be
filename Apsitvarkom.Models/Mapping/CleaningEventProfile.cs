using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.Models.Mapping;

/// <summary>
/// Implements a profile for AutoMapper that allows creating maps required for conversion of class
/// <see cref="CleaningEvent" /> related objects (Requests, Responses, Business objects).
/// </summary>
public class CleaningEventProfile : Profile
{
    public CleaningEventProfile()
    {
        MapRequests();
        MapResponses();
    }

    private void MapRequests()
    {
        CreateMap<ObjectIdentifyRequest, CleaningEvent>(MemberList.None)
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));
        CreateMap<CleaningEventCreateRequest, CleaningEvent>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.PollutedLocation, opt => opt.Ignore());
        CreateMap<CleaningEventUpdateRequest, CleaningEvent>()
            .ForMember(x => x.StartTime, opt => opt.PreCondition(src => src.StartTime is not null))
            .ForMember(x => x.Notes, opt => opt.PreCondition(src => src.Notes is not null))
            .ForMember(x => x.PollutedLocationId, opt => opt.Ignore())
            .ForMember(x => x.PollutedLocation, opt => opt.Ignore());
    }

    private void MapResponses()
    {
        CreateMap<CleaningEvent, CleaningEventResponse>();
    }
}