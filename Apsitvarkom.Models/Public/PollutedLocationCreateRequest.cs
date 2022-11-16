using FluentValidation;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationCreateRequest
{
    public int? Radius { get; set; }

    public SeverityLevel? Severity { get; set; }

    public int? Progress { get; set; }

    public string? Notes { get; set; }

    public CoordinatesCreateRequest? Coordinates { get; set; }
}

public class PollutedLocationCreateRequestValidator : AbstractValidator<PollutedLocationCreateRequest>
{
    public PollutedLocationCreateRequestValidator(IValidator<CoordinatesCreateRequest> coordinatesCreateRequestValidator)
    {
        RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(l => l.Severity).NotNull();
        RuleFor(l => l.Progress).NotNull().InclusiveBetween(0, 100);

        RuleFor(l => l.Coordinates).NotNull();
        RuleFor(l => l.Coordinates!).SetValidator(coordinatesCreateRequestValidator).When(l => l.Coordinates is not null);
    }
}
