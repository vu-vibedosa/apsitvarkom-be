using FluentValidation;

namespace Apsitvarkom.Models.DTO;

/// <summary>DTO equivalent of <see cref="Coordinates"/>.</summary>
public struct CoordinatesDTO
{
    public double? Longitude;
    public double? Latitude;
}

/// <summary>
/// Data Transfer Object for <see cref="Location" />
/// </summary>
public class LocationDTO
{
    /// <summary>Property equivalent of <see cref="Location.Coordinates"/>.</summary>
    public CoordinatesDTO? Coordinates { get; set; }
}

/// <summary>Validator for <see cref="LocationDTO"/>.</summary>
public class LocationDTOValidator : AbstractValidator<CoordinatesDTO>
{
    public LocationDTOValidator()
    {
        RuleFor(dto => dto.Longitude).NotNull();
        RuleFor(dto => dto.Latitude).NotNull();
    }
}