using FluentValidation;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationResponse
{
    public Guid Id { get; init; }

    public LocationResponse Location { get; set; } = new();

    public int Progress { get; set; }

    public int Radius { get; set; }

    public SeverityLevel Severity { get; set; }

    public DateTime Spotted { get; init; }

    public string? Notes { get; set; }

    public List<TidyingEventResponse> Events { get; set; } = new();
}

public class PollutedLocationResponseValidator : AbstractValidator<PollutedLocationResponse>
{
    public PollutedLocationResponseValidator(IValidator<LocationResponse> locationResponseValidator)
    {
        RuleFor(l => l.Location).SetValidator(locationResponseValidator);

        RuleFor(l => l.Radius).GreaterThanOrEqualTo(1);
        RuleFor(l => l.Progress).InclusiveBetween(0, 100);
    }
}
