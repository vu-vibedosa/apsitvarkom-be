using FluentValidation;

namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="PollutedLocation" />
/// </summary>
public class PollutedLocationDTO
{
    /// <summary>Property equivalent to <see cref="PollutedLocation.Id" /></summary>
    public string? Id { get; init; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Location" /></summary>
    public LocationDTO? Location { get; set; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Radius" /></summary>
    public int? Radius { get; set; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Severity" /></summary>
    public string? Severity { get; set; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Spotted" /></summary>
    public string? Spotted { get; init; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Progress" /></summary>
    public int? Progress { get; set; }

    /// <summary>Property equivalent to <see cref="PollutedLocation.Notes" /></summary>
    public string? Notes { get; set; }
}

public class PollutedLocationDTOValidator: AbstractValidator<PollutedLocationDTO>
{
    public PollutedLocationDTOValidator()
    {
        RuleFor(dto => dto.Id).NotNull();
        RuleFor(dto => dto.Location).NotNull();
        RuleFor(dto => dto.Location!.Value).SetValidator(new LocationDTOValidator()).When(dto => dto.Location.HasValue);
        RuleFor(dto => dto.Radius).NotNull();
        RuleFor(dto => dto.Severity).NotNull();
        RuleFor(dto => dto.Spotted).NotNull();
        RuleFor(dto => dto.Progress).NotNull();
        RuleFor(dto => dto.Notes).NotNull();
    }
}