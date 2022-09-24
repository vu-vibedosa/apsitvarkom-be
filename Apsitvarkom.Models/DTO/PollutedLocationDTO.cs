using FluentValidation;
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
        RuleFor(dto => dto.Id).NotEmpty().Must(BeValidGuid);
        RuleFor(dto => dto.Radius).NotNull().Must(BeAboveMininumRadius);
        RuleFor(dto => dto.Severity).NotEmpty().Must(BeDefinedSeverity);
        RuleFor(dto => dto.Spotted).NotEmpty().Must(BeDateOfValidFormat);
        RuleFor(dto => dto.Progress).NotNull().Must(BeValidProgress);
        RuleFor(dto => dto.Notes).Must(BeValidNotes);

        // Make sure Location itself is not null
        RuleFor(dto => dto.Location).NotNull();
        // Validate all Location fields using its validator if it has a value
        RuleFor(dto => dto.Location!.Value).SetValidator(new LocationDTOValidator()).When(dto => dto.Location.HasValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool BeValidGuid(string? id)
    {
        return Guid.TryParse(id, out var parsedId);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool BeAboveMininumRadius(int? radius)
    {
        return radius >= 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="severity"></param>
    /// <returns></returns>
    private bool BeDefinedSeverity(string? severity)
    {
        return Enum.IsDefined(typeof(LocationSeverityLevel), severity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private bool BeDateOfValidFormat(string? date)
    {
        DateTime tempDate; 
        return DateTime.TryParse(date, out tempDate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="progress"></param>
    /// <returns></returns>
    private bool BeValidProgress(int? progress)
    {
        return progress is >= 0 and <= 100;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notes"></param>
    /// <returns></returns>
    private bool BeValidNotes(string? notes)
    {
        return notes is not null && notes.Length <= 200;
    }


}