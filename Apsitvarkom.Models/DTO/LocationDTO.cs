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
        RuleFor(dto => dto.Longitude).NotNull().WithState(dto => new ArgumentNullException(nameof(dto.Longitude), $"{nameof(dto.Longitude)} is null")).
            Must(BeValidLongitude).WithState(dto => new ArgumentOutOfRangeException(nameof(dto.Longitude), $"Longitude cannot be less than -180.0 or exceed 180.0 degrees, but was {dto.Longitude}."));

        RuleFor(dto => dto.Latitude).NotNull().WithState(dto => new ArgumentNullException(nameof(dto.Latitude), $"{nameof(dto.Latitude)} is null")).
            Must(BeValidLatitude).WithState(dto => new ArgumentOutOfRangeException(nameof(dto.Latitude), $"Latitude cannot be less than -90.0 or exceed 90.0 degrees, but was {dto.Latitude}."));
    }

    /// <summary>
    /// Checks whether Longitude is between valid limits
    /// </summary>
    /// <param name="longitude">Longitude to be checked.</param>
    /// <returns>Flag of whether the validation was successfull.</returns>
    private bool BeValidLongitude(double? longitude)
    {
        return longitude is >= -180.0 and <= 180.0;
    }

    /// <summary>
    /// Checks whether Latitude is between valid limits.
    /// </summary>
    /// <param name="latitude">Latitude to be checked.</param>
    /// <returns>Flag of whether the validation was successfull.</returns>
    private bool BeValidLatitude(double? latitude)
    {
        return latitude is >= -90.0 and <= 90.0;
    }
}