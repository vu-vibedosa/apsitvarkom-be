using FluentValidation;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationCreateRequest
{
    public LocationCreateRequest? Location { get; set; }

    public int? Radius { get; set; }

    public SeverityLevel? Severity { get; set; }

    public string? Notes { get; set; }
}

public class PollutedLocationCreateRequestValidator : AbstractValidator<PollutedLocationCreateRequest>
{
    public PollutedLocationCreateRequestValidator(IValidator<LocationCreateRequest> locationCreateRequestValidator)
    {
        RuleFor(l => l.Radius).NotNull().GreaterThanOrEqualTo(1);
        RuleFor(l => l.Severity).NotNull();

        RuleFor(l => l.Location).NotNull();
        RuleFor(l => l.Location!).SetValidator(locationCreateRequestValidator).When(l => l.Location is not null);
    }
}
