using FluentValidation;

namespace Apsitvarkom.Models.DTO;

/// <summary>DTO equivalent of <see cref="Coordinates"/>.</summary>
public class CoordinatesDTO
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class CoordinatesDTOValidator : AbstractValidator<CoordinatesDTO>
{
    public CoordinatesDTOValidator()
    {
        RuleFor(dto => dto.Latitude).NotNull();
        RuleFor(dto => dto.Longitude).NotNull();
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
    public LocationDTOValidator(CoordinatesDTOValidator coordinatesDtoValidator)
    {
        RuleFor(dto => dto.Coordinates).NotNull();
        RuleFor(dto => dto.Coordinates!).SetValidator(coordinatesDtoValidator).When(dto => dto.Coordinates is not null);
    }
}