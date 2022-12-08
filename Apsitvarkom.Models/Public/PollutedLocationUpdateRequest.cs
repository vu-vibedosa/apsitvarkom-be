using FluentValidation;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationUpdateRequest
{
    public Guid? Id { get; set; }

    public int? Radius { get; set; }

    public SeverityLevel? Severity { get; set; }

    public string? Notes { get; set; }
}

public class PollutedLocationUpdateRequestValidator : AbstractValidator<PollutedLocationUpdateRequest>
{
    public PollutedLocationUpdateRequestValidator()
    {
       RuleFor(l => l.Id).NotNull();
       RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
       RuleFor(l => l.Severity).NotNull();
    }
}