using System.Globalization;
using Apsitvarkom.Models.Public;
using AutoMapper;

namespace Apsitvarkom.Models.Mapping;

/// <summary>
/// Implements a profile for AutoMapper that allows creating maps required for conversion of class
/// <see cref="PollutedLocationDTO" /> objects to <see cref="PollutedLocation" /> used in the business logic and vice-versa.
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
        CreateMap<CoordinatesGetRequest, Coordinates>();
    }

    private void MapResponses()
    {
        CreateMap<PollutedLocation, PollutedLocationGetResponse>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Spotted, opt => opt
                .MapFrom(src => src.Spotted.ToString("o", CultureInfo.InvariantCulture)));

        CreateMap<Location, LocationGetResponse>();
        CreateMap<Coordinates, CoordinatesGetResponse>();
    }
}