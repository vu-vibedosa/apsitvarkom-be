using FluentValidation;
using System;
using static Apsitvarkom.Models.Enumerations;

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
        RuleFor(dto => dto.Id).NotEmpty().WithState(dto => new MissingFieldException(nameof(dto.Id), $"{nameof(dto.Id)} is empty"))
            .Must(BeValidGuid).WithState(dto => new FormatException($"{dto.Id} is of Invalid Guid format"));

        RuleFor(dto => dto.Radius).NotNull().WithState(dto => new ArgumentNullException(nameof(dto.Radius), $"{nameof(dto.Radius)} is null"))
            .Must(BeAboveMininumRadius).WithState(dto => new ArgumentOutOfRangeException(nameof(dto.Radius), $"{nameof(dto.Radius)} can only be higher than 0, but was {dto.Radius}."));

        RuleFor(dto => dto.Severity).NotEmpty().WithState(dto => new MissingFieldException(nameof(dto.Severity), $"{nameof(dto.Severity)} is empty"))
            .Must(BeDefinedSeverity).WithState(dto => new ArgumentOutOfRangeException(nameof(dto.Severity), $"The pollution severity state {dto.Severity} is not defined."));

        RuleFor(dto => dto.Spotted).NotEmpty().WithState(dto => new MissingFieldException(nameof(dto.Spotted), $"{nameof(dto.Spotted)} is empty"))
            .Must(BeDateOfValidFormat).WithState(dto => new FormatException($"{nameof(dto.Spotted)} date is of invalid date format"));

        RuleFor(dto => dto.Progress).NotNull().WithState(dto => new ArgumentNullException(nameof(dto.Progress), $"{nameof(dto.Progress)} is null"))
            .Must(BeValidProgress).WithState(dto => new ArgumentOutOfRangeException(nameof(dto.Progress), $"Progress is depicted in percentages and can only have values between 0 and 100, but was {dto.Progress}."));

        RuleFor(dto => dto.Notes).Must(BeValidNotes).WithState(dto => new FormatException($"{nameof(dto.Notes)} has more than 200 characters"));

        // Make sure Location itself is not null
        RuleFor(dto => dto.Location).NotNull();
        // Validate all Location fields using its validator if it has a value
        RuleFor(dto => dto.Location!.Value).SetValidator(new LocationDTOValidator()).When(dto => dto.Location.HasValue);
    }

    /// <summary>
    /// Checks whether the input Id is parseable as <see cref="Guid"/>.
    /// </summary>
    /// <param name="id">String to be parsed.</param>
    /// <returns>Flag of whether the validation was successful.</returns>
    private bool BeValidGuid(string? id)
    {
        return Guid.TryParse(id, out _);
    }

    /// <summary>
    /// Checks whether area Radius is above the minimum required size.
    /// </summary>
    /// <param name="radius"><see cref="PollutedLocation.Radius"/> that is being checked.</param>
    /// <returns>Flag of whether the validation was successful.</returns>
    private bool BeAboveMininumRadius(int? radius)
    {
        return radius >= 1;
    }

    /// <summary>
    /// Checks whether Severity is defined in the <see cref="LocationSeverityLevel"/>.
    /// </summary>
    /// <param name="severity">Key that is being checked in the Enum.</param>
    /// <returns>Flag of whether the validation was successful</returns>
    private bool BeDefinedSeverity(string? severity)
    {
        return Enum.IsDefined(typeof(LocationSeverityLevel), severity);
    }

    /// <summary>
    /// Checks whether input string is parseable as Date.
    /// </summary>
    /// <param name="date">Input to be parsed.</param>
    /// <returns>Flag of whether the validation was successful</returns>
    private bool BeDateOfValidFormat(string? date)
    {
        return DateTime.TryParse(date, out _);
    }

    /// <summary>
    /// Checks whether Progress within its boundaries.
    /// </summary>
    /// <param name="progress">Input to be checked.</param>
    /// <returns>Flag of whether the validation was successful</returns>
    private bool BeValidProgress(int? progress)
    {
        return progress is >= 0 and <= 100;
    }

    /// <summary>
    /// Checks whether Notes do not exceed maximum length.
    /// </summary>
    /// <param name="notes">Input that is checked.</param>
    /// <returns>Flag of whether the validation was successfull.</returns>
    private bool BeValidNotes(string? notes)
    {
        return notes is not null && notes.Length <= 200; // This number is subject to change, we should discuss this later on.
    }
}