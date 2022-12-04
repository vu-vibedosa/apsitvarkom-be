using FluentValidation;

namespace Apsitvarkom.Models.Public;

public class CleaningEventCreateRequest
{
    public Guid? PollutedLocationId { get; set; }

    public DateTime? StartTime { get; set; }

    public string? Notes { get; set; }
}

public class CleaningEventCreateRequestValidator : AbstractValidator<CleaningEventCreateRequest>
{
    public CleaningEventCreateRequestValidator()
    {
        RuleFor(l => l.StartTime).NotNull().GreaterThan(DateTime.UtcNow);
        RuleFor(l => l.PollutedLocationId).NotNull();
    }
}
