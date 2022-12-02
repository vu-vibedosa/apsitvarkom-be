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
        CreateMap<ObjectIdentifyRequest, CleaningEvent>(MemberList.None).
            ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));
    }

    private void MapResponses()
    {
        CreateMap<CleaningEvent, CleaningEventResponse>();
    }
}