using FluentValidation;

namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="Location" />
/// </summary>
public struct LocationDTO
{
    /// <summary>Property equivalent to <see cref="Location.Longitude" /></summary>
    public double? Longitude { get; set; }

    /// <summary>Property equivalent to <see cref="Location.Latitude" /></summary>
    public double? Latitude { get; set; }
}

public class LocationDTOValidator : AbstractValidator<LocationDTO>
{
    public LocationDTOValidator()
    {
        RuleFor(dto => dto.Longitude).NotNull().Must(BeValidLongitude);
        RuleFor(dto => dto.Latitude).NotNull().Must(BeValidLatitude);
    }

    private bool BeValidLongitude(double? longitude)
    {
        return longitude is not null && longitude is >= -180.0 and <= 180.0;
    }

    private bool BeValidLatitude(double? latitude)
    {
        return latitude is not null && latitude is >= -90.0 and <= 90.0;
    }
}