using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.Models.Mapping;

/// <summary>
/// Implements a profile for AutoMapper that allows creating maps required for conversion of class
/// <see cref="TidyingEvent" /> related objects (Requests, Responses, Business objects).
/// </summary>
public class TidyingEventProfile : Profile
{
    public TidyingEventProfile()
    {
        MapRequests();
        MapResponses();
    }

    private void MapRequests()
    {

    }

    private void MapResponses()
    {
        CreateMap<TidyingEvent, TidyingEventResponse>();
    }
}