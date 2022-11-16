using FluentValidation;
using static Apsitvarkom.Models.PollutedLocation;

namespace Apsitvarkom.Models.Public;

public class PollutedLocationGetResponse
{
    public Guid Id { get; init; }

    public int Radius { get; set; }

    public SeverityLevel Severity { get; set; }

    public DateTime Spotted { get; init; }

    public int Progress { get; set; }

    public string? Notes { get; set; }

    public LocationGetResponse Location { get; set; } = new();
}

public class PollutedLocationGetResponseValidator : AbstractValidator<PollutedLocationGetResponse>
{
    public PollutedLocationGetResponseValidator(IValidator<LocationGetResponse> locationGetResponseValidator)
    {
        RuleFor(l => l.Location).SetValidator(locationGetResponseValidator);

        RuleFor(l => l.Radius).GreaterThanOrEqualTo(1);
        RuleFor(l => l.Progress).InclusiveBetween(0, 100);
    }
}
