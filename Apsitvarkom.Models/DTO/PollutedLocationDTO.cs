using FluentValidation;
using System.Globalization;

namespace Apsitvarkom.Models.DTO;

/// <summary>
/// Data Transfer Object for <see cref="PollutedLocation" />
/// </summary>
public class PollutedLocationDTO : LocationDTO
{
    /// <summary>Property equivalent to <see cref="PollutedLocation.Id" /></summary>
    public string? Id { get; init; }

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
/// <summary>
/// Validator for <see cref="PollutedLocationDTO"/>.
/// </summary>
public class PollutedLocationDTOValidator : AbstractValidator<PollutedLocationDTO>
{
    public PollutedLocationDTOValidator()
    {
        // Include base class validator
        Include(new LocationDTOValidator());

        RuleFor(dto => dto.Id).NotEmpty().Must(BeValidGuid);
        RuleFor(dto => dto.Radius).NotNull();
        RuleFor(dto => dto.Severity).NotEmpty().IsEnumName(typeof(Enumerations.LocationSeverityLevel));
        RuleFor(dto => dto.Spotted).NotEmpty().Must(BeDateOfValidFormat);
        RuleFor(dto => dto.Progress).NotNull();
        RuleFor(dto => dto.Notes).NotNull();
    }

    /// <summary>
    /// Checks whether the input Id is parseable as <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">String to be parsed.</param>
    /// <returns>Flag of whether the validation was successful.</returns>
    private bool BeValidGuid(string? id) => Guid.TryParse(id, out _);

    /// <summary>
    /// Checks whether input string is parseable as <see cref="DateTime"/>.
    /// </summary>
    /// <param name="date">Input to be parsed.</param>
    /// <returns>Flag of whether the validation was successful</returns>
    private bool BeDateOfValidFormat(string? date) => DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
}