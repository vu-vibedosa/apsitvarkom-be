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
        CreateMap<CleaningEventCreateRequest, CleaningEvent>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }

    private void MapResponses()
    {
        CreateMap<CleaningEvent, CleaningEventResponse>();
    }
}