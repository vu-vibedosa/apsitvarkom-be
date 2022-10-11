using FluentValidation;
using static Apsitvarkom.Models.Enumerations;

namespace Apsitvarkom.Models;

/// <summary>
/// Class for storing captured polluted location records.
/// </summary>
public class PollutedLocation : Location
{
    /// <summary>Unique identifier of the given record.</summary>
    public Guid Id { get; init; }

    /// <summary>Rough size estimate of the given record area in meters from the center.</summary>
    public int Radius { get; set; }

    /// <summary>Estimated current pollution level of the record.</summary>
    public LocationSeverityLevel Severity { get; set; }

    /// <summary><see cref="DateTime" /> of when the record was created.</summary>
    public DateTime Spotted { get; init; }

    /// <summary>Current progress of the record's cleaning process in percentages.</summary>
    public int Progress { get; set; }

    /// <summary>Additional information about the record.</summary>
    public string Notes { get; set; } = null!;
}

public class PollutedLocationValidator : AbstractValidator<PollutedLocation>
{
    public PollutedLocationValidator()
    {
        // There is no need to perform validation on Severity, Id and Spotted since they are parsed/validated in DTO before mapping.
        RuleFor(data => data.Radius).GreaterThanOrEqualTo(1);
        RuleFor(data => data.Severity).IsInEnum();
        RuleFor(data => data.Progress).InclusiveBetween(0, 100);
        // Validate all Location fields using its validator if it has a Value
        RuleFor(data => data.Coordinates).SetValidator(new LocationValidator());
    }
}