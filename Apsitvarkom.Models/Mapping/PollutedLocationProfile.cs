using System.Globalization;
using Apsitvarkom.Models.DTO;
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
        // PollutedLocationDTO to PollutedLocation
        CreateMap<PollutedLocationDTO, PollutedLocation>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.Spotted, opt => opt
                .MapFrom(src => DateTime.Parse(src.Spotted, CultureInfo.InvariantCulture)));

        // LocationDTO to Location
        CreateMap<LocationDTO, Location>();


        // PollutedLocation to PollutedLocationDTO
        CreateMap<PollutedLocation, PollutedLocationDTO>()
            .ForMember(dest => dest.Id, opt => opt
                .MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Spotted, opt => opt
                .MapFrom(src => src.Spotted.ToString(CultureInfo.InvariantCulture)));

        // Location to LocationDTO
        CreateMap<Location, LocationDTO>();
    }
}