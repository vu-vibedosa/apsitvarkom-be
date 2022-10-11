using FluentValidation;

namespace Apsitvarkom.Models.DTO;

/// <summary>DTO equivalent of <see cref="Coordinates"/>.</summary>
public struct CoordinatesDTO
{
    public double? Longitude;
    public double? Latitude;
}

public class CoordinatesDTOValidator : AbstractValidator<CoordinatesDTO>
{
    public CoordinatesDTOValidator()
    {
        RuleFor(dto => dto.Longitude).NotNull();
        RuleFor(dto => dto.Latitude).NotNull();
    }
}

/// <summary>
/// Data Transfer Object for <see cref="Location" />
/// </summary>
public class LocationDTO
{
    /// <summary>Property equivalent of <see cref="Location.Coordinates"/>.</summary>
    public CoordinatesDTO? Coordinates { get; set; }
}

public class LocationDTOValidator : AbstractValidator<LocationDTO>
{
    public LocationDTOValidator()
    {
        RuleFor(dto => dto.Coordinates).NotNull();
        RuleFor(dto => dto.Coordinates!.Value).SetValidator(new CoordinatesDTOValidator()).When(dto => dto.Coordinates.HasValue);
    }
}